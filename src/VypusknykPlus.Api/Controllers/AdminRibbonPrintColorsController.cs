using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/admin/ribbon-print-colors")]
[Authorize(Roles = "Admin")]
public class AdminRibbonPrintColorsController : ControllerBase
{
    private readonly AppDbContext _db;
    public AdminRibbonPrintColorsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.RibbonPrintColors
            .IgnoreQueryFilters()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.SortOrder).ThenBy(c => c.Id)
            .Select(c => Map(c))
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var c = await _db.RibbonPrintColors.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        return c is null ? NotFound() : Ok(Map(c));
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaveRibbonPrintColorRequest req)
    {
        var c = new RibbonPrintColor
        {
            Name          = req.Name,
            Slug          = req.Slug,
            Hex           = req.Hex,
            PriceModifier = req.PriceModifier,
            IsActive      = req.IsActive,
            SortOrder     = req.SortOrder,
            CreatedAt     = DateTime.UtcNow,
            UpdatedAt     = DateTime.UtcNow,
        };
        _db.RibbonPrintColors.Add(c);
        await _db.SaveChangesAsync();
        return Ok(Map(c));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, SaveRibbonPrintColorRequest req)
    {
        var c = await _db.RibbonPrintColors.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (c is null) return NotFound();

        c.Name          = req.Name;
        c.Slug          = req.Slug;
        c.Hex           = req.Hex;
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
        var c = await _db.RibbonPrintColors.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (c is null) return NotFound();
        c.IsDeleted = true;
        c.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static RibbonPrintColorResponse Map(RibbonPrintColor c) => new()
    {
        Id            = c.Id,
        Name          = c.Name,
        Slug          = c.Slug,
        Hex           = c.Hex,
        PriceModifier = c.PriceModifier,
        IsActive      = c.IsActive,
        SortOrder     = c.SortOrder,
    };
}
