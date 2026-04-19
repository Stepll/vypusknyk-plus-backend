using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/products")]
public class AdminProductsController : ControllerBase
{
    private readonly IAdminService _admin;

    public AdminProductsController(IAdminService admin) => _admin = admin;

    [HttpGet]
    public async Task<ActionResult<PagedResponse<AdminProductResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return Ok(await _admin.GetProductsAsync(page, pageSize));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<AdminProductDetailResponse>> GetById(long id)
    {
        var product = await _admin.GetProductAsync(id);
        if (product is null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<AdminProductDetailResponse>> Create([FromBody] SaveProductRequest request)
    {
        var product = await _admin.CreateProductAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<AdminProductDetailResponse>> Update(long id, [FromBody] SaveProductRequest request)
    {
        var product = await _admin.UpdateProductAsync(id, request);
        return Ok(product);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _admin.DeleteProductAsync(id);
        return NoContent();
    }
}
