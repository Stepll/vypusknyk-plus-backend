using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/admin/badge-sizes")]
[Authorize(Roles = "Admin")]
public class AdminBadgeSizesController : ControllerBase
{
    private readonly AppDbContext _db;
    public AdminBadgeSizesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.BadgeSizes
            .IgnoreQueryFilters()
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.SortOrder).ThenBy(s => s.Id)
            .Select(s => Map(s))
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var s = await _db.BadgeSizes.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        return s is null ? NotFound() : Ok(Map(s));
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaveBadgeSizeRequest req)
    {
        var s = new BadgeSize
        {
            Name          = req.Name,
            Diameter      = req.Diameter,
            PriceModifier = req.PriceModifier,
            IsActive      = req.IsActive,
            SortOrder     = req.SortOrder,
            CreatedAt     = DateTime.UtcNow,
            UpdatedAt     = DateTime.UtcNow,
        };
        _db.BadgeSizes.Add(s);
        await _db.SaveChangesAsync();
        return Ok(Map(s));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, SaveBadgeSizeRequest req)
    {
        var s = await _db.BadgeSizes.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (s is null) return NotFound();

        s.Name          = req.Name;
        s.Diameter      = req.Diameter;
        s.PriceModifier = req.PriceModifier;
        s.IsActive      = req.IsActive;
        s.SortOrder     = req.SortOrder;
        s.UpdatedAt     = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(Map(s));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var s = await _db.BadgeSizes.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (s is null) return NotFound();
        s.IsDeleted = true;
        s.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static BadgeSizeResponse Map(BadgeSize s) => new()
    {
        Id            = s.Id,
        Name          = s.Name,
        Diameter      = s.Diameter,
        PriceModifier = s.PriceModifier,
        IsActive      = s.IsActive,
        SortOrder     = s.SortOrder,
    };
}
