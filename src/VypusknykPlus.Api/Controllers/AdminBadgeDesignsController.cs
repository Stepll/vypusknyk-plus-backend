using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/badge-designs")]
public class AdminBadgeDesignsController : ControllerBase
{
    private readonly IBadgeDesignService _service;

    public AdminBadgeDesignsController(IBadgeDesignService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<PagedResponse<AdminBadgeDesignResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return Ok(await _service.GetAllAdminAsync(page, pageSize));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<AdminBadgeDesignResponse>> GetById(long id)
    {
        var result = await _service.GetAdminByIdAsync(id);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpGet("by-user/{userId:long}")]
    public async Task<ActionResult<List<AdminBadgeDesignResponse>>> GetByUser(long userId)
    {
        return Ok(await _service.GetByUserIdAsync(userId));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _service.DeleteAdminAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
