using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services.AuditLogs;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/admin/audit-logs")]
[Authorize(Roles = "Admin")]
public class AdminAuditLogsController : ControllerBase
{
    private readonly IAuditLogService _auditLogs;

    public AdminAuditLogsController(IAuditLogService auditLogs) => _auditLogs = auditLogs;

    [HttpGet]
    public async Task<ActionResult<PagedResponse<AuditLogResponse>>> GetAll(
        [FromQuery] string[]? entityTypes,
        [FromQuery] long? entityId,
        [FromQuery] long? adminId,
        [FromQuery] string? action,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        return Ok(await _auditLogs.GetLogsAsync(entityTypes, entityId, adminId, action, from, to, page, pageSize));
    }
}
