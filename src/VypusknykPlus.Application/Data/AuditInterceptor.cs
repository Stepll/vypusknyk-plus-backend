using System.Text.Json;
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
        typeof(ProductCategory), typeof(ProductSubcategory)
    ];

    private static readonly HashSet<string> ExcludedFields =
    [
        "IsDeleted", "CreatedAt", "UpdatedAt", "PasswordHash"
    ];

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
            .Where(p => p.EntityId > 0)
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
        var (adminId, adminName) = GetAdmin();
        var pending = new List<PendingAudit>();

        foreach (var entry in context.ChangeTracker.Entries()
            .Where(e => TrackedTypes.Contains(e.Metadata.ClrType)
                     && e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
        {
            string action;
            string? changesJson = null;

            var entityId = entry.Properties
                .FirstOrDefault(p => p.Metadata.Name == "Id")
                ?.CurrentValue is long id ? id : 0L;

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
                        .Select(p => new
                        {
                            field = p.Metadata.Name,
                            old = p.OriginalValue?.ToString(),
                            @new = p.CurrentValue?.ToString()
                        })
                        .ToList();

                    if (changes.Count == 0) continue;
                    changesJson = JsonSerializer.Serialize(changes);
                }
            }

            pending.Add(new PendingAudit
            {
                EntityRef = entry.State == EntityState.Added ? entry.Entity : null,
                EntityType = entry.Metadata.ClrType.Name,
                EntityId = entry.State == EntityState.Added ? 0 : entityId,
                Action = action,
                ChangesJson = changesJson,
                AdminId = adminId,
                AdminName = adminName
            });
        }

        return pending;
    }

    private (long? adminId, string adminName) GetAdmin() => _adminProvider.GetCurrent();

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
