using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Application.Services.AuditLogs;

public interface IAuditLogService
{
    Task<PagedResponse<AuditLogResponse>> GetLogsAsync(
        string[]? entityTypes,
        long? entityId,
        long? adminId,
        string? action,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize);
}
