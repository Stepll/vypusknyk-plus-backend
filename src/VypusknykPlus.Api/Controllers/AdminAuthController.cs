using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/admin/auth")]
public class AdminAuthController : ControllerBase
{
    private readonly IAdminAuthService _auth;

    public AdminAuthController(IAdminAuthService auth) => _auth = auth;

    [HttpPost("login")]
    public async Task<ActionResult<AdminAuthResponse>> Login([FromBody] AdminLoginRequest request)
    {
        var response = await _auth.LoginAsync(request);
        return Ok(response);
    }
}
