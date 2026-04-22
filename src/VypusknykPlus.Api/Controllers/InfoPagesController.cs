using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/info")]
public class InfoPagesController(IInfoPageService infoPages) : ControllerBase
{
    [HttpGet("{slug}")]
    public async Task<ActionResult<InfoPageResponse>> GetBySlug(string slug)
    {
        var page = await infoPages.GetBySlugAsync(slug);
        if (page is null) return NotFound();
        return Ok(page);
    }
}
