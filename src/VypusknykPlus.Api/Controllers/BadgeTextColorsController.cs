using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/badge-text-colors")]
public class BadgeTextColorsController : ControllerBase
{
    private readonly AppDbContext _db;
    public BadgeTextColorsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var items = await _db.BadgeTextColors
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder).ThenBy(x => x.Id)
            .Select(x => new BadgeTextColorResponse
            {
                Id = x.Id, Name = x.Name, Hex = x.Hex,
                PriceModifier = x.PriceModifier, IsActive = x.IsActive, SortOrder = x.SortOrder,
            })
            .ToListAsync();
        return Ok(items);
    }
}
