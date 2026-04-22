using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/info-pages")]
public class AdminInfoPagesController(IInfoPageService infoPages) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<InfoPageResponse>>> GetAll()
        => Ok(await infoPages.GetAllAsync());

    [HttpPut("{slug}")]
    public async Task<ActionResult<InfoPageResponse>> Update(string slug, [FromBody] UpdateInfoPageRequest request)
        => Ok(await infoPages.UpdateBySlugAsync(slug, request));
}
