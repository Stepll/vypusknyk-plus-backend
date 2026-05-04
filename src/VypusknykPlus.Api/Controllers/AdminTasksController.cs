using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs.Tasks;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/tasks")]
public class AdminTasksController(ITaskService tasks) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AdminTaskResponse>>> GetAll() =>
        Ok(await tasks.GetAdminTasksAsync());

    [HttpPost]
    public async Task<ActionResult<AdminTaskResponse>> Create([FromBody] SaveTaskRequest request) =>
        Ok(await tasks.CreateTaskAsync(request));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<AdminTaskResponse>> Update(long id, [FromBody] SaveTaskRequest request) =>
        Ok(await tasks.UpdateTaskAsync(id, request));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await tasks.DeleteTaskAsync(id);
        return NoContent();
    }
}
