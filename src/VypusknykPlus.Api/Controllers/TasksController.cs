using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs.Tasks;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/tasks")]
public class TasksController(ITaskService tasks) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<PublicTaskResponse>>> GetAll()
    {
        long? userId = User.Identity?.IsAuthenticated == true ? GetUserIdOrNull() : null;
        return Ok(await tasks.GetPublicTasksAsync(userId));
    }

    private long? GetUserIdOrNull()
    {
        var val = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return val != null ? long.Parse(val) : null;
    }
}
