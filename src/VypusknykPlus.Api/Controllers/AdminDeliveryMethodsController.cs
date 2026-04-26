using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/delivery-methods")]
public class AdminDeliveryMethodsController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminDeliveryMethodsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<DeliveryMethodResponse>>> GetAll()
    {
        var methods = await _db.DeliveryMethods
            .AsNoTracking()
            .OrderBy(m => m.Id)
            .ToListAsync();

        return Ok(methods.Select(Map).ToList());
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<DeliveryMethodResponse>> GetById(long id)
    {
        var method = await _db.DeliveryMethods.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
        return method is null ? NotFound() : Ok(Map(method));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<DeliveryMethodResponse>> Update(long id, [FromBody] UpdateDeliveryMethodRequest request)
    {
        var method = await _db.DeliveryMethods.FirstOrDefaultAsync(m => m.Id == id);
        if (method is null) return NotFound();

        method.IsEnabled = request.IsEnabled;
        method.Settings = request.Settings;
        method.CheckoutFields = JsonSerializer.Serialize(request.CheckoutFields);
        method.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(Map(method));
    }

    private static DeliveryMethodResponse Map(DeliveryMethod m)
    {
        var fields = new List<DeliveryCheckoutFieldDto>();
        try { fields = JsonSerializer.Deserialize<List<DeliveryCheckoutFieldDto>>(m.CheckoutFields, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? []; }
        catch { /* return empty list on malformed JSON */ }

        return new DeliveryMethodResponse
        {
            Id = m.Id,
            Name = m.Name,
            Slug = m.Slug,
            IsEnabled = m.IsEnabled,
            Settings = m.Settings,
            CheckoutFields = fields,
        };
    }
}
