using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/page-content")]
public class PageContentController(IPageContentService pageContent) : ControllerBase
{
    [HttpGet("{slug}")]
    public async Task<IActionResult> Get(string slug)
    {
        var data = await pageContent.GetDataAsync(slug);
        if (data is null) return NotFound();
        return Content(data, "application/json");
    }
}
