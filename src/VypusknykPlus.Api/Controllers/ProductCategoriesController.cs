using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/product-categories")]
public class ProductCategoriesController : ControllerBase
{
    private readonly IProductCategoryService _service;

    public ProductCategoriesController(IProductCategoryService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<ProductCategoryResponse>>> GetAll()
        => Ok(await _service.GetAllAsync());
}
