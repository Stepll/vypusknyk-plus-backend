using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/admin/ribbon-colors")]
[Authorize(Roles = "Admin")]
public class AdminRibbonColorsController : ControllerBase
{
    private readonly AppDbContext _db;
    public AdminRibbonColorsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.RibbonColors
            .IgnoreQueryFilters()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Id)
            .Select(c => Map(c))
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var c = await _db.RibbonColors.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        return c is null ? NotFound() : Ok(Map(c));
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaveRibbonColorRequest req)
    {
        var c = new RibbonColor
        {
            Name          = req.Name,
            Slug          = req.Slug,
            Hex           = req.Hex,
            SecondaryHex  = string.IsNullOrWhiteSpace(req.SecondaryHex) ? null : req.SecondaryHex,
            PriceModifier = req.PriceModifier,
            IsActive      = req.IsActive,
            SortOrder     = req.SortOrder,
            CreatedAt     = DateTime.UtcNow,
            UpdatedAt     = DateTime.UtcNow,
        };
        _db.RibbonColors.Add(c);
        await _db.SaveChangesAsync();
        return Ok(Map(c));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, SaveRibbonColorRequest req)
    {
        var c = await _db.RibbonColors.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (c is null) return NotFound();

        c.Name          = req.Name;
        c.Slug          = req.Slug;
        c.Hex           = req.Hex;
        c.SecondaryHex  = string.IsNullOrWhiteSpace(req.SecondaryHex) ? null : req.SecondaryHex;
        c.PriceModifier = req.PriceModifier;
        c.IsActive      = req.IsActive;
        c.SortOrder     = req.SortOrder;
        c.UpdatedAt     = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(Map(c));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var c = await _db.RibbonColors.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (c is null) return NotFound();
        c.IsDeleted = true;
        c.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static RibbonColorResponse Map(RibbonColor c) => new()
    {
        Id            = c.Id,
        Name          = c.Name,
        Slug          = c.Slug,
        Hex           = c.Hex,
        SecondaryHex  = c.SecondaryHex,
        PriceModifier = c.PriceModifier,
        IsActive      = c.IsActive,
        SortOrder     = c.SortOrder,
    };
}
