using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/warehouse")]
public class AdminWarehouseController(IWarehouseService warehouse) : ControllerBase
{
    [HttpGet("categories")]
    public async Task<ActionResult<List<StockCategoryResponse>>> GetCategories()
    {
        return Ok(await warehouse.GetCategoriesAsync());
    }

    [HttpGet("stats")]
    public async Task<ActionResult<WarehouseStatsResponse>> GetStats()
    {
        return Ok(await warehouse.GetStatsAsync());
    }

    [HttpGet("products")]
    public async Task<ActionResult<PagedResponse<StockProductSummary>>> GetProducts(
        [FromQuery] WarehouseProductsQuery query)
    {
        return Ok(await warehouse.GetProductsAsync(query));
    }

    [HttpGet("products/{id:long}")]
    public async Task<ActionResult<StockProductDetail>> GetProduct(long id)
    {
        var product = await warehouse.GetProductDetailAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost("transactions")]
    public async Task<ActionResult<StockTransactionResponse>> AddTransaction(
        [FromBody] CreateStockTransactionRequest request)
    {
        var result = await warehouse.AddTransactionAsync(request);
        return Created(string.Empty, result);
    }
}
