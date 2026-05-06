using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/admin/badge-text-colors")]
[Authorize(Roles = "Admin")]
public class AdminBadgeTextColorsController : ControllerBase
{
    private readonly AppDbContext _db;
    public AdminBadgeTextColorsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.BadgeTextColors
            .IgnoreQueryFilters().Where(x => !x.IsDeleted)
            .OrderBy(x => x.SortOrder).ThenBy(x => x.Id)
            .Select(x => Map(x)).ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var x = await _db.BadgeTextColors.IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        return x is null ? NotFound() : Ok(Map(x));
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaveBadgeTextColorRequest req)
    {
        var x = new BadgeTextColor
        {
            Name = req.Name, Hex = req.Hex, PriceModifier = req.PriceModifier,
            IsActive = req.IsActive, SortOrder = req.SortOrder,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow,
        };
        _db.BadgeTextColors.Add(x);
        await _db.SaveChangesAsync();
        return Ok(Map(x));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, SaveBadgeTextColorRequest req)
    {
        var x = await _db.BadgeTextColors.IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (x is null) return NotFound();
        x.Name = req.Name; x.Hex = req.Hex; x.PriceModifier = req.PriceModifier;
        x.IsActive = req.IsActive; x.SortOrder = req.SortOrder;
        x.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(Map(x));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var x = await _db.BadgeTextColors.IgnoreQueryFilters()
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (x is null) return NotFound();
        x.IsDeleted = true; x.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static BadgeTextColorResponse Map(BadgeTextColor x) => new()
    {
        Id = x.Id, Name = x.Name, Hex = x.Hex, PriceModifier = x.PriceModifier,
        IsActive = x.IsActive, SortOrder = x.SortOrder,
    };
}
