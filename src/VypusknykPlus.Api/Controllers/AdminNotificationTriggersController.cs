using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs.Notifications;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/admin/notification-triggers")]
[Authorize(Roles = "Admin")]
public class AdminNotificationTriggersController : ControllerBase
{
    private readonly INotificationService _notifications;

    public AdminNotificationTriggersController(INotificationService notifications)
        => _notifications = notifications;

    [HttpGet]
    public async Task<ActionResult<List<NotificationTriggerConfigResponse>>> GetAll()
        => Ok(await _notifications.GetTriggerConfigsAsync());

    [HttpPut("{triggerType}")]
    public async Task<IActionResult> Update(string triggerType, [FromBody] UpdateNotificationTriggerConfigRequest request)
    {
        await _notifications.UpdateTriggerConfigAsync(triggerType, request);
        return NoContent();
    }
}
