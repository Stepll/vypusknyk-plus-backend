using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/designs")]
public class AdminDesignsController : ControllerBase
{
    private readonly IAdminService _admin;

    public AdminDesignsController(IAdminService admin) => _admin = admin;

    [HttpGet]
    public async Task<ActionResult<PagedResponse<AdminSavedDesignResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return Ok(await _admin.GetSavedDesignsAsync(page, pageSize));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<AdminSavedDesignDetailResponse>> GetById(long id)
    {
        var result = await _admin.GetSavedDesignAsync(id);
        if (result is null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            await _admin.DeleteSavedDesignAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
