using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/payment-methods")]
public class PaymentMethodsController(AppDbContext db, IAppSettingsService appSettings) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<PaymentMethodResponse>>> GetActive()
    {
        var settings = await appSettings.GetPublicAsync();
        var onlinePaymentEnabled = !settings.TryGetValue("online_payment_enabled", out var v) || v == "true";

        var methods = await db.PaymentMethods
            .AsNoTracking()
            .Where(m => m.IsEnabled && (onlinePaymentEnabled || m.Slug != "online"))
            .OrderBy(m => m.Id)
            .ToListAsync();

        return Ok(methods.Select(m => new PaymentMethodResponse
        {
            Id = m.Id,
            Name = m.Name,
            Slug = m.Slug,
            IsEnabled = m.IsEnabled,
        }).ToList());
    }
}
