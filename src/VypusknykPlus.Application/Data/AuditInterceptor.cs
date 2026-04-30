using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentAdminProvider _adminProvider;
    private List<PendingAudit> _pending = [];

    private static readonly HashSet<Type> TrackedTypes =
    [
        typeof(Order), typeof(Product), typeof(User), typeof(Admin),
        typeof(Role), typeof(Delivery), typeof(Supplier),
        typeof(ProductCategory), typeof(ProductSubcategory),
        typeof(StockProduct),
        typeof(DeliveryMethod), typeof(PaymentMethod), typeof(OrderStatus),
        typeof(NotificationTriggerConfig),
        typeof(RibbonColor), typeof(RibbonMaterial), typeof(RibbonPrintColor),
        typeof(RibbonFont), typeof(RibbonPrintType), typeof(RibbonEmblem),
        typeof(ConstructorIncompatibility), typeof(ConstructorForcedText),
    ];

    private static readonly HashSet<string> ExcludedFields =
    [
        "IsDeleted", "CreatedAt", "UpdatedAt", "PasswordHash"
    ];

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public AuditInterceptor(ICurrentAdminProvider adminProvider)
    {
        _adminProvider = adminProvider;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        _pending = CaptureChanges(eventData.Context!);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData, int result, CancellationToken ct = default)
    {
        var context = eventData.Context!;

        foreach (var p in _pending.Where(p => p.Action == "Create" && p.EntityRef is not null))
        {
            var idProp = p.EntityRef!.GetType().GetProperty("Id");
            if (idProp?.GetValue(p.EntityRef) is long id)
                p.EntityId = id;
        }

        var logs = _pending
            .Where(p => p.EntityId > 0 || p.EntityType == nameof(NotificationTriggerConfig))
            .Select(p => new AuditLog
            {
                AdminId = p.AdminId,
                AdminName = p.AdminName,
                EntityType = p.EntityType,
                EntityId = p.EntityId,
                Action = p.Action,
                ChangesJson = p.ChangesJson,
                CreatedAt = DateTime.UtcNow
            })
            .ToList();

        if (logs.Count > 0)
        {
            context.Set<AuditLog>().AddRange(logs);
            await context.SaveChangesAsync(ct);
        }

        return result;
    }

    private List<PendingAudit> CaptureChanges(DbContext context)
    {
        var admin = GetAdmin();
        var pending = new List<PendingAudit>();

        // Standard entity tracking
        foreach (var entry in context.ChangeTracker.Entries()
            .Where(e => TrackedTypes.Contains(e.Metadata.ClrType)
                     && e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
        {
            var entityId = GetEntityId(entry);
            string action;
            string? changesJson = null;

            if (entry.State == EntityState.Deleted)
            {
                action = "Delete";
            }
            else if (entry.State == EntityState.Added)
            {
                action = "Create";
                var snapshot = entry.Properties
                    .Where(p => !ExcludedFields.Contains(p.Metadata.Name) && p.CurrentValue is not null)
                    .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue!.ToString());
                changesJson = JsonSerializer.Serialize(snapshot);
            }
            else
            {
                var isDeletedProp = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "IsDeleted");
                if (isDeletedProp?.IsModified == true && isDeletedProp.CurrentValue is true)
                {
                    action = "Delete";
                }
                else
                {
                    action = "Update";
                    var changes = entry.Properties
                        .Where(p => p.IsModified && !ExcludedFields.Contains(p.Metadata.Name))
                        .Select(p => new FieldChange(p.Metadata.Name, p.OriginalValue?.ToString(), p.CurrentValue?.ToString()))
                        .ToList();

                    if (changes.Count == 0) continue;
                    changesJson = JsonSerializer.Serialize(changes, JsonOpts);
                }
            }

            pending.Add(new PendingAudit
            {
                EntityRef = entry.State == EntityState.Added ? entry.Entity : null,
                EntityType = entry.Metadata.ClrType.Name,
                EntityId = entry.State == EntityState.Added ? 0 : entityId,
                Action = action,
                ChangesJson = changesJson,
                AdminId = admin.AdminId,
                AdminName = admin.AdminName
            });
        }

        // Merge ConstructorIncompatibilityTarget child changes into parent entries
        MergeChildChanges<ConstructorIncompatibilityTarget>(
            context, pending, admin,
            parentTypeName: nameof(ConstructorIncompatibility),
            getRuleId: e => GetPropLong(e, "RuleId"),
            getSlugOrValue: e => GetPropString(e, "SlugB"),
            fieldName: "SlugsB");

        // Merge ConstructorForcedTextValue child changes into parent entries
        MergeChildChanges<ConstructorForcedTextValue>(
            context, pending, admin,
            parentTypeName: nameof(ConstructorForcedText),
            getRuleId: e => GetPropLong(e, "RuleId"),
            getSlugOrValue: e => GetPropString(e, "Value"),
            fieldName: "Values");

        return pending;
    }

    private void MergeChildChanges<TChild>(
        DbContext context,
        List<PendingAudit> pending,
        (long? AdminId, string AdminName) admin,
        string parentTypeName,
        Func<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry, long> getRuleId,
        Func<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry, string> getSlugOrValue,
        string fieldName)
    {
        var childEntries = context.ChangeTracker.Entries()
            .Where(e => e.Metadata.ClrType == typeof(TChild)
                     && e.State is EntityState.Added or EntityState.Deleted)
            .ToList();

        if (childEntries.Count == 0) return;

        foreach (var group in childEntries.GroupBy(getRuleId).Where(g => g.Key > 0))
        {
            var ruleId = group.Key;
            var oldValues = group.Where(e => e.State == EntityState.Deleted)
                .Select(e => getSlugOrValue(e)).OrderBy(s => s).ToList();
            var newValues = group.Where(e => e.State == EntityState.Added)
                .Select(e => getSlugOrValue(e)).OrderBy(s => s).ToList();

            var slugChange = new FieldChange(fieldName,
                oldValues.Count > 0 ? string.Join(", ", oldValues) : null,
                newValues.Count > 0 ? string.Join(", ", newValues) : null);

            var parentEntry = pending.FirstOrDefault(p =>
                p.EntityType == parentTypeName && p.EntityId == ruleId && p.Action == "Update");

            if (parentEntry is not null)
            {
                var existing = parentEntry.ChangesJson is not null
                    ? JsonSerializer.Deserialize<List<FieldChange>>(parentEntry.ChangesJson, JsonOpts) ?? []
                    : [];
                existing.Add(slugChange);
                parentEntry.ChangesJson = JsonSerializer.Serialize(existing, JsonOpts);
            }
            else
            {
                pending.Add(new PendingAudit
                {
                    EntityType = parentTypeName,
                    EntityId = ruleId,
                    Action = "Update",
                    ChangesJson = JsonSerializer.Serialize(new[] { slugChange }, JsonOpts),
                    AdminId = admin.AdminId,
                    AdminName = admin.AdminName
                });
            }
        }
    }

    private static long GetEntityId(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        => entry.Properties.FirstOrDefault(p => p.Metadata.Name == "Id")?.CurrentValue is long id ? id : 0L;

    private static long GetPropLong(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry, string name)
        => entry.Properties.FirstOrDefault(p => p.Metadata.Name == name)?.CurrentValue is long v ? v :
           entry.Properties.FirstOrDefault(p => p.Metadata.Name == name)?.OriginalValue is long ov ? ov : 0L;

    private static string GetPropString(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry, string name)
        => entry.Properties.FirstOrDefault(p => p.Metadata.Name == name)?.CurrentValue?.ToString()
        ?? entry.Properties.FirstOrDefault(p => p.Metadata.Name == name)?.OriginalValue?.ToString()
        ?? "";

    private (long? AdminId, string AdminName) GetAdmin() => _adminProvider.GetCurrent();

    private sealed record FieldChange(
        [property: JsonPropertyName("field")] string Field,
        [property: JsonPropertyName("old")] string? Old,
        [property: JsonPropertyName("new")] string? New);

    private sealed class PendingAudit
    {
        public object? EntityRef { get; set; }
        public string EntityType { get; set; } = "";
        public long EntityId { get; set; }
        public string Action { get; set; } = "";
        public string? ChangesJson { get; set; }
        public long? AdminId { get; set; }
        public string AdminName { get; set; } = "";
    }
}
