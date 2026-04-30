using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Application.Services.AuditLogs;

public class AuditLogService : IAuditLogService
{
    private readonly AppDbContext _db;

    public AuditLogService(AppDbContext db) => _db = db;

    public async Task<PagedResponse<AuditLogResponse>> GetLogsAsync(
        string[]? entityTypes,
        long? entityId,
        long? adminId,
        string? action,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize)
    {
        var query = _db.AuditLogs.AsNoTracking();

        if (entityTypes is { Length: > 0 })
            query = query.Where(a => entityTypes.Contains(a.EntityType));

        if (entityId.HasValue)
            query = query.Where(a => a.EntityId == entityId.Value);

        if (adminId.HasValue)
            query = query.Where(a => a.AdminId == adminId.Value);

        if (!string.IsNullOrEmpty(action))
            query = query.Where(a => a.Action == action);

        if (from.HasValue)
            query = query.Where(a => a.CreatedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(a => a.CreatedAt <= to.Value);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new AuditLogResponse
            {
                Id = a.Id,
                AdminId = a.AdminId,
                AdminName = a.AdminName,
                EntityType = a.EntityType,
                EntityId = a.EntityId,
                Action = a.Action,
                ChangesJson = a.ChangesJson,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();

        return new PagedResponse<AuditLogResponse>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }
}
