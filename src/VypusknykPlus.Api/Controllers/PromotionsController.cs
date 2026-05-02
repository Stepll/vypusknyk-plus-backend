using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs.Promotions;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/promotions")]
public class PromotionsController(IPromotionService promotions) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<PublicPromotionResponse>>> GetAll() =>
        Ok(await promotions.GetPublicPromotionsAsync());

    [HttpGet("my-cards")]
    [Authorize]
    public async Task<ActionResult<List<PromoCodeCardResponse>>> GetMyCards()
    {
        var userId = GetUserId();
        return Ok(await promotions.GetMyCardsAsync(userId));
    }

    [HttpPost("activate")]
    [Authorize]
    public async Task<ActionResult<PromoCodeCardResponse>> Activate([FromBody] ActivatePromoCodeRequest request)
    {
        var userId = GetUserId();
        var card = await promotions.ActivatePromoCodeAsync(request.Code, userId);
        return Ok(card);
    }

    [HttpPost("calculate")]
    public async Task<ActionResult<CalculateDiscountResponse>> Calculate([FromBody] CalculateDiscountRequest request)
    {
        long? userId = User.Identity?.IsAuthenticated == true ? GetUserIdOrNull() : null;
        return Ok(await promotions.CalculateDiscountAsync(request.OrderTotal, request.UserPromoCardId, userId));
    }

    private long GetUserId() =>
        long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException());

    private long? GetUserIdOrNull() =>
        long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
}
