using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Products;
using VypusknykPlus.Application.Services;
using VypusknykPlus.Application.DTOs.AppSettings;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController(IProductService productService, IAppSettingsService appSettings) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProductResponse>>> GetAll(
        [FromQuery] ProductQueryParams query)
    {
        var settings = await appSettings.GetPublicAsync();
        if (settings.TryGetValue("catalog_enabled", out var v) && v == "false")
            return StatusCode(503, new { message = "Каталог тимчасово недоступний." });

        var response = await productService.GetAllAsync(query);
        return Ok(response);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ProductResponse>> GetById(long id)
    {
        var response = await productService.GetByIdAsync(id);
        if (response is null) return NotFound();
        return Ok(response);
    }

    [HttpPost("{id:long}/image")]
    [Authorize]
    // TODO: Replace [Authorize] with [Authorize(Roles = "Admin")] once role-based auth is implemented.
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
    public async Task<ActionResult<ProductResponse>> UploadImage(
        long id,
        IFormFile image)
    {
        if (image is null || image.Length == 0)
            return BadRequest(new { message = "No image file provided." });

        string[] allowed = ["image/jpeg", "image/png", "image/webp"];
        if (!allowed.Contains(image.ContentType))
            return BadRequest(new { message = "Only JPEG, PNG, and WebP images are supported." });

        await using var stream = image.OpenReadStream();
        var response = await productService.UploadImageAsync(id, stream, image.ContentType);
        return Ok(response);
    }
}
