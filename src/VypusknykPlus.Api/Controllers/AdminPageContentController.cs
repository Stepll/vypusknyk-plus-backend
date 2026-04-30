using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/page-content")]
public class AdminPageContentController(IPageContentService pageContent) : ControllerBase
{
    [HttpGet("{slug}")]
    public async Task<IActionResult> Get(string slug)
    {
        var data = await pageContent.GetDataAsync(slug);
        if (data is null) return NotFound();
        return Content(data, "application/json");
    }

    [HttpPut("{slug}")]
    public async Task<IActionResult> Update(string slug, [FromBody] object body)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(body);
        var data = await pageContent.UpsertDataAsync(slug, json);
        return Content(data, "application/json");
    }

    [HttpPost("{slug}/images")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<ActionResult<UploadImageResponse>> UploadImage(string slug, [FromQuery] string field, IFormFile file)
    {
        if (file is null || file.Length == 0) return BadRequest("No file");
        var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
        if (!allowed.Contains(file.ContentType)) return BadRequest("Only jpeg/png/webp allowed");

        await using var stream = file.OpenReadStream();
        var url = await pageContent.UploadImageAsync(slug, field, stream, file.ContentType);
        return Ok(new UploadImageResponse(url));
    }
}

public record UploadImageResponse(string Url);
