using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/users")]
public class AdminUsersController : ControllerBase
{
    private readonly IAdminService _admin;

    public AdminUsersController(IAdminService admin) => _admin = admin;

    [HttpGet]
    public async Task<ActionResult<PagedResponse<AdminUserResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return Ok(await _admin.GetUsersAsync(page, pageSize));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<AdminUserDetailResponse>> GetById(long id)
    {
        var user = await _admin.GetUserAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPatch("{id:long}/info")]
    public async Task<ActionResult<AdminUserDetailResponse>> PatchInfo(long id, [FromBody] PatchUserInfoRequest request)
    {
        var user = await _admin.PatchUserInfoAsync(id, request);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPatch("{id:long}/verification")]
    public async Task<ActionResult<AdminUserDetailResponse>> PatchVerification(long id, [FromBody] PatchUserVerificationRequest request)
    {
        var user = await _admin.PatchUserVerificationAsync(id, request);
        return user is null ? NotFound() : Ok(user);
    }
}
