using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/product-categories")]
public class AdminProductCategoriesController : ControllerBase
{
    private readonly IProductCategoryService _service;

    public AdminProductCategoriesController(IProductCategoryService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<ProductCategoryResponse>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpPost]
    public async Task<ActionResult<ProductCategoryResponse>> Create([FromBody] SaveProductCategoryRequest request)
        => Ok(await _service.CreateCategoryAsync(request));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<ProductCategoryResponse>> Update(long id, [FromBody] SaveProductCategoryRequest request)
        => Ok(await _service.UpdateCategoryAsync(id, request));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _service.DeleteCategoryAsync(id);
        return NoContent();
    }

    [HttpPost("{categoryId:long}/subcategories")]
    public async Task<ActionResult<ProductSubcategoryResponse>> CreateSubcategory(
        long categoryId, [FromBody] SaveProductSubcategoryRequest request)
        => Ok(await _service.CreateSubcategoryAsync(categoryId, request));

    [HttpPut("subcategories/{id:long}")]
    public async Task<ActionResult<ProductSubcategoryResponse>> UpdateSubcategory(
        long id, [FromBody] SaveProductSubcategoryRequest request)
        => Ok(await _service.UpdateSubcategoryAsync(id, request));

    [HttpDelete("subcategories/{id:long}")]
    public async Task<IActionResult> DeleteSubcategory(long id)
    {
        await _service.DeleteSubcategoryAsync(id);
        return NoContent();
    }
}
