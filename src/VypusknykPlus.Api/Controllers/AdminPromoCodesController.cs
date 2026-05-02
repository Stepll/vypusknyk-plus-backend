using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs.Promotions;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/promo-codes")]
public class AdminPromoCodesController(IPromotionService promotions) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AdminPromoCodeResponse>>> GetAll() =>
        Ok(await promotions.GetAdminPromoCodesAsync());

    [HttpPost]
    public async Task<ActionResult<AdminPromoCodeResponse>> Create([FromBody] SavePromoCodeRequest request) =>
        Ok(await promotions.CreatePromoCodeAsync(request));

    [HttpPut("{id:long}")]
    public async Task<ActionResult<AdminPromoCodeResponse>> Update(long id, [FromBody] SavePromoCodeRequest request) =>
        Ok(await promotions.UpdatePromoCodeAsync(id, request));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await promotions.DeletePromoCodeAsync(id);
        return NoContent();
    }
}
