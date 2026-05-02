using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs.Promotions;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/promotions")]
public class AdminPromotionsController(IPromotionService promotions) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AdminPromotionResponse>>> GetAll() =>
        Ok(await promotions.GetAdminPromotionsAsync());

    [HttpPost]
    public async Task<ActionResult<AdminPromotionResponse>> Create([FromBody] SavePromotionRequest request) =>
        Ok(await promotions.CreatePromotionAsync(request));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<AdminPromotionResponse>> Update(long id, [FromBody] SavePromotionRequest request) =>
        Ok(await promotions.UpdatePromotionAsync(id, request));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await promotions.DeletePromotionAsync(id);
        return NoContent();
    }
}
