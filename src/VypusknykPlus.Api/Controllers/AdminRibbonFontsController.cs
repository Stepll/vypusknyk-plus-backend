using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/admin/ribbon-fonts")]
[Authorize(Roles = "Admin")]
public class AdminRibbonFontsController : ControllerBase
{
    private readonly AppDbContext _db;
    public AdminRibbonFontsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.RibbonFonts
            .IgnoreQueryFilters()
            .Where(f => !f.IsDeleted)
            .OrderBy(f => f.SortOrder).ThenBy(f => f.Id)
            .Select(f => Map(f))
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var f = await _db.RibbonFonts.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        return f is null ? NotFound() : Ok(Map(f));
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaveRibbonFontRequest req)
    {
        var f = new RibbonFont
        {
            Name       = req.Name,
            Slug       = req.Slug,
            FontFamily = req.FontFamily,
            ImportUrl  = string.IsNullOrWhiteSpace(req.ImportUrl) ? null : req.ImportUrl,
            IsActive   = req.IsActive,
            SortOrder  = req.SortOrder,
            CreatedAt  = DateTime.UtcNow,
            UpdatedAt  = DateTime.UtcNow,
        };
        _db.RibbonFonts.Add(f);
        await _db.SaveChangesAsync();
        return Ok(Map(f));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, SaveRibbonFontRequest req)
    {
        var f = await _db.RibbonFonts.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (f is null) return NotFound();

        f.Name       = req.Name;
        f.Slug       = req.Slug;
        f.FontFamily = req.FontFamily;
        f.ImportUrl  = string.IsNullOrWhiteSpace(req.ImportUrl) ? null : req.ImportUrl;
        f.IsActive   = req.IsActive;
        f.SortOrder  = req.SortOrder;
        f.UpdatedAt  = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(Map(f));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var f = await _db.RibbonFonts.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (f is null) return NotFound();
        f.IsDeleted = true;
        f.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static RibbonFontResponse Map(RibbonFont f) => new()
    {
        Id         = f.Id,
        Name       = f.Name,
        Slug       = f.Slug,
        FontFamily = f.FontFamily,
        ImportUrl  = f.ImportUrl,
        IsActive   = f.IsActive,
        SortOrder  = f.SortOrder,
    };
}
