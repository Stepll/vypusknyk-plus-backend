using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/admins")]
public class AdminAdminsController : ControllerBase
{
    private readonly IAdminService _admin;

    public AdminAdminsController(IAdminService admin) => _admin = admin;

    [HttpGet]
    public async Task<ActionResult<PagedResponse<AdminAdminResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return Ok(await _admin.GetAdminsAsync(page, pageSize));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<AdminAdminDetailResponse>> GetById(long id)
    {
        var admin = await _admin.GetAdminDetailAsync(id);
        return admin is null ? NotFound() : Ok(admin);
    }

    [HttpPost]
    public async Task<ActionResult<AdminAdminDetailResponse>> Create([FromBody] CreateAdminRequest request)
    {
        var admin = await _admin.CreateAdminAsync(request);
        return Created(string.Empty, admin);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _admin.DeleteAdminAsync(id);
        return NoContent();
    }
}
