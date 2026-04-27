using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/payment-methods")]
public class PaymentMethodsController : ControllerBase
{
    private readonly AppDbContext _db;

    public PaymentMethodsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<PaymentMethodResponse>>> GetActive()
    {
        var methods = await _db.PaymentMethods
            .AsNoTracking()
            .Where(m => m.IsEnabled)
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
