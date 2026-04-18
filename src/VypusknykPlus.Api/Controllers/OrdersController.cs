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

    private long GetUserId() => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string GetUserEmail() => User.FindFirstValue(ClaimTypes.Email)!;

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<OrderResponse>> Create([FromBody] CreateOrderRequest request)
    {
        long? userId = User.Identity?.IsAuthenticated == true ? GetUserId() : null;
        var response = await _orderService.CreateAsync(userId, request);
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

    [HttpGet("guest/{guestToken}")]
    [AllowAnonymous]
    public async Task<ActionResult<OrderListResponse>> GetGuestOrders(string guestToken)
    {
        var response = await _orderService.GetGuestOrdersAsync(guestToken);
        return Ok(response);
    }

    [HttpPost("claim")]
    public async Task<IActionResult> ClaimGuestOrders([FromBody] ClaimGuestOrdersRequest request)
    {
        await _orderService.ClaimGuestOrdersAsync(GetUserId(), GetUserEmail(), request.GuestToken);
        return NoContent();
    }
}
