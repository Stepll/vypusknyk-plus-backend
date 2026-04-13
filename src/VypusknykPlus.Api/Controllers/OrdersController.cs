using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VypusknykPlus.Application.DTOs.Orders;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create([FromBody] CreateOrderRequest request)
    {
        var response = await _orderService.CreateAsync(GetUserId(), request);
        return Created(string.Empty, response);
    }

    [HttpGet]
    public async Task<ActionResult<OrderListResponse>> GetUserOrders()
    {
        var response = await _orderService.GetUserOrdersAsync(GetUserId());
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetById(Guid id)
    {
        var response = await _orderService.GetByIdAsync(GetUserId(), id);
        if (response is null) return NotFound();
        return Ok(response);
    }
}
