using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs.Notifications;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/admin/notifications")]
[Authorize(Roles = "Admin")]
public class AdminNotificationsController : ControllerBase
{
    private readonly INotificationService _notifications;

    public AdminNotificationsController(INotificationService notifications)
        => _notifications = notifications;

    [HttpGet]
    public async Task<ActionResult<List<AdminNotificationDto>>> GetMy([FromQuery] int limit = 50)
    {
        var adminId = GetAdminId();
        return Ok(await _notifications.GetMyNotificationsAsync(adminId, limit));
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        var adminId = GetAdminId();
        return Ok(await _notifications.GetUnreadCountAsync(adminId));
    }

    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkRead(long id)
    {
        var adminId = GetAdminId();
        await _notifications.MarkReadAsync(id, adminId);
        return NoContent();
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        var adminId = GetAdminId();
        await _notifications.MarkAllReadAsync(adminId);
        return NoContent();
    }

    private long GetAdminId()
        => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
}
