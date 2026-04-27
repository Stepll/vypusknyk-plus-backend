using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/payment-methods")]
public class AdminPaymentMethodsController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminPaymentMethodsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<PaymentMethodResponse>>> GetAll()
    {
        var methods = await _db.PaymentMethods
            .AsNoTracking()
            .OrderBy(m => m.Id)
            .ToListAsync();

        return Ok(methods.Select(Map).ToList());
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<PaymentMethodResponse>> GetById(long id)
    {
        var method = await _db.PaymentMethods.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        return method is null ? NotFound() : Ok(Map(method));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<PaymentMethodResponse>> Update(long id, [FromBody] UpdatePaymentMethodRequest request)
    {
        var method = await _db.PaymentMethods.FirstOrDefaultAsync(m => m.Id == id);
        if (method is null) return NotFound();

        method.IsEnabled = request.IsEnabled;
        method.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(Map(method));
    }

    private static PaymentMethodResponse Map(PaymentMethod m) => new()
    {
        Id = m.Id,
        Name = m.Name,
        Slug = m.Slug,
        IsEnabled = m.IsEnabled,
    };
}
