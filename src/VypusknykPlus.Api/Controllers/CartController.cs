using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VypusknykPlus.Application.DTOs.Cart;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private long GetUserId() => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<List<CartItemResponse>>> GetCart()
    {
        var items = await _cartService.GetUserCartAsync(GetUserId());
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<CartItemResponse>> AddItem([FromBody] AddCartItemRequest request)
    {
        var response = await _cartService.AddAsync(GetUserId(), request);
        return Created(string.Empty, response);
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<CartItemResponse>> UpdateQty(Guid id, [FromBody] UpdateCartItemRequest request)
    {
        var response = await _cartService.UpdateQtyAsync(GetUserId(), id, request);
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> RemoveItem(Guid id)
    {
        await _cartService.DeleteAsync(GetUserId(), id);
        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> ClearCart()
    {
        await _cartService.ClearAsync(GetUserId());
        return NoContent();
    }
}
