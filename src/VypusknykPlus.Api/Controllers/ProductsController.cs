using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Products;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProductResponse>>> GetAll(
        [FromQuery] ProductQueryParams query)
    {
        var response = await _productService.GetAllAsync(query);
        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductResponse>> GetById(int id)
    {
        var response = await _productService.GetByIdAsync(id);
        if (response is null) return NotFound();
        return Ok(response);
    }

    [HttpPost("{id:int}/image")]
    [Authorize]
    // TODO: Replace [Authorize] with [Authorize(Roles = "Admin")] once role-based auth is implemented.
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
    public async Task<ActionResult<ProductResponse>> UploadImage(
        int id,
        IFormFile image)
    {
        if (image is null || image.Length == 0)
            return BadRequest(new { message = "No image file provided." });

        string[] allowed = ["image/jpeg", "image/png", "image/webp"];
        if (!allowed.Contains(image.ContentType))
            return BadRequest(new { message = "Only JPEG, PNG, and WebP images are supported." });

        await using var stream = image.OpenReadStream();
        var response = await _productService.UploadImageAsync(id, stream, image.ContentType);
        return Ok(response);
    }
}
