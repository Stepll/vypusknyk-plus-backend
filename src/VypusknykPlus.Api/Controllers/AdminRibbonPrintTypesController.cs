using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/admin/ribbon-print-types")]
[Authorize(Roles = "Admin")]
public class AdminRibbonPrintTypesController : ControllerBase
{
    private readonly AppDbContext _db;
    public AdminRibbonPrintTypesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.RibbonPrintTypes
            .IgnoreQueryFilters()
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.SortOrder).ThenBy(p => p.Id)
            .Select(p => Map(p))
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var p = await _db.RibbonPrintTypes.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        return p is null ? NotFound() : Ok(Map(p));
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaveRibbonPrintTypeRequest req)
    {
        var p = new RibbonPrintType
        {
            Name          = req.Name,
            Slug          = req.Slug,
            PriceModifier = req.PriceModifier,
            IsActive      = req.IsActive,
            SortOrder     = req.SortOrder,
            CreatedAt     = DateTime.UtcNow,
            UpdatedAt     = DateTime.UtcNow,
        };
        _db.RibbonPrintTypes.Add(p);
        await _db.SaveChangesAsync();
        return Ok(Map(p));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, SaveRibbonPrintTypeRequest req)
    {
        var p = await _db.RibbonPrintTypes.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (p is null) return NotFound();

        p.Name          = req.Name;
        p.Slug          = req.Slug;
        p.PriceModifier = req.PriceModifier;
        p.IsActive      = req.IsActive;
        p.SortOrder     = req.SortOrder;
        p.UpdatedAt     = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(Map(p));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var p = await _db.RibbonPrintTypes.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (p is null) return NotFound();
        p.IsDeleted = true;
        p.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static RibbonPrintTypeResponse Map(RibbonPrintType p) => new()
    {
        Id            = p.Id,
        Name          = p.Name,
        Slug          = p.Slug,
        PriceModifier = p.PriceModifier,
        IsActive      = p.IsActive,
        SortOrder     = p.SortOrder,
    };
}
