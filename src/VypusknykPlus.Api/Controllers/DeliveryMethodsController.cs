using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/delivery-methods")]
public class DeliveryMethodsController : ControllerBase
{
    private readonly AppDbContext _db;

    public DeliveryMethodsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<DeliveryMethodResponse>>> GetActive()
    {
        var methods = await _db.DeliveryMethods
            .AsNoTracking()
            .Where(m => m.IsEnabled)
            .OrderBy(m => m.Id)
            .ToListAsync();

        return Ok(methods.Select(m =>
        {
            var fields = new List<DeliveryCheckoutFieldDto>();
            try { fields = JsonSerializer.Deserialize<List<DeliveryCheckoutFieldDto>>(m.CheckoutFields, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? []; }
            catch { }

            return new DeliveryMethodResponse
            {
                Id = m.Id,
                Name = m.Name,
                Slug = m.Slug,
                IsEnabled = m.IsEnabled,
                Settings = m.Settings,
                CheckoutFields = fields,
            };
        }).ToList());
    }
}
