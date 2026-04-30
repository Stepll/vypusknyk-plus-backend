using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VypusknykPlus.Application.DTOs.Notifications;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/admin/notification-triggers")]
[Authorize(Roles = "Admin")]
public class AdminNotificationTriggersController : ControllerBase
{
    private readonly INotificationService _notifications;
    private readonly IAdminService _admin;
    private readonly string? _superAdminEmail;

    public AdminNotificationTriggersController(INotificationService notifications, IAdminService admin, IConfiguration config)
    {
        _notifications = notifications;
        _admin = admin;
        _superAdminEmail = config["Admin:Email"];
    }

    [HttpGet]
    public async Task<ActionResult<List<NotificationTriggerConfigResponse>>> GetAll()
        => Ok(await _notifications.GetTriggerConfigsAsync());

    [HttpGet("recipients")]
    public async Task<ActionResult<List<NotificationAdminRecipientDto>>> GetRecipients()
    {
        var dbAdmins = await _admin.GetAdminsAsync(1, 200);
        var result = new List<NotificationAdminRecipientDto>();

        if (!string.IsNullOrWhiteSpace(_superAdminEmail))
            result.Add(new NotificationAdminRecipientDto { Id = 0, FullName = "Super Admin", Email = _superAdminEmail });

        result.AddRange(dbAdmins.Items.Select(a => new NotificationAdminRecipientDto
        {
            Id = a.Id,
            FullName = a.FullName,
            Email = a.Email,
        }));

        return Ok(result);
    }

    [HttpPut("{triggerType}")]
    public async Task<IActionResult> Update(string triggerType, [FromBody] UpdateNotificationTriggerConfigRequest request)
    {
        await _notifications.UpdateTriggerConfigAsync(triggerType, request);
        return NoContent();
    }
}
