using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/roles")]
public class AdminRolesController : ControllerBase
{
    private readonly IAdminRoleService _roles;

    public AdminRolesController(IAdminRoleService roles) => _roles = roles;

    [HttpGet]
    public async Task<ActionResult<List<RoleResponse>>> GetAll()
        => Ok(await _roles.GetRolesAsync());

    [HttpGet("{id:long}")]
    public async Task<ActionResult<RoleResponse>> GetById(long id)
    {
        var role = await _roles.GetRoleAsync(id);
        return role is null ? NotFound() : Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<RoleResponse>> Create([FromBody] CreateRoleRequest request)
    {
        var role = await _roles.CreateRoleAsync(request);
        return Created(string.Empty, role);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<RoleResponse>> Update(long id, [FromBody] UpdateRoleRequest request)
    {
        var role = await _roles.UpdateRoleAsync(id, request);
        return Ok(role);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _roles.DeleteRoleAsync(id);
        return NoContent();
    }
}
