using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs.AppSettings;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
public class AppSettingsController(IAppSettingsService appSettings) : ControllerBase
{
    [HttpGet("api/v1/settings")]
    public async Task<ActionResult<Dictionary<string, string>>> GetPublic()
    {
        var result = await appSettings.GetPublicAsync();
        return Ok(result);
    }

    [HttpGet("api/v1/admin/settings")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<AppSettingResponse>>> GetAll()
    {
        var result = await appSettings.GetAllAsync();
        return Ok(result);
    }

    [HttpPut("api/v1/admin/settings")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateMany([FromBody] List<UpdateAppSettingRequest> updates)
    {
        await appSettings.UpdateManyAsync(updates);
        return NoContent();
    }
}
