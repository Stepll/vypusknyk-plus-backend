using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/admin/ribbon-materials")]
[Authorize(Roles = "Admin")]
public class AdminRibbonMaterialsController : ControllerBase
{
    private readonly AppDbContext _db;
    public AdminRibbonMaterialsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.RibbonMaterials
            .IgnoreQueryFilters()
            .Where(m => !m.IsDeleted)
            .OrderBy(m => m.SortOrder).ThenBy(m => m.Id)
            .Select(m => Map(m))
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var m = await _db.RibbonMaterials.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        return m is null ? NotFound() : Ok(Map(m));
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaveRibbonMaterialRequest req)
    {
        var m = new RibbonMaterial
        {
            Name          = req.Name,
            Slug          = req.Slug,
            PriceModifier = req.PriceModifier,
            IsActive      = req.IsActive,
            SortOrder     = req.SortOrder,
            CreatedAt     = DateTime.UtcNow,
            UpdatedAt     = DateTime.UtcNow,
        };
        _db.RibbonMaterials.Add(m);
        await _db.SaveChangesAsync();
        return Ok(Map(m));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, SaveRibbonMaterialRequest req)
    {
        var m = await _db.RibbonMaterials.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (m is null) return NotFound();

        m.Name          = req.Name;
        m.Slug          = req.Slug;
        m.PriceModifier = req.PriceModifier;
        m.IsActive      = req.IsActive;
        m.SortOrder     = req.SortOrder;
        m.UpdatedAt     = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(Map(m));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var m = await _db.RibbonMaterials.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (m is null) return NotFound();
        m.IsDeleted = true;
        m.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static RibbonMaterialResponse Map(RibbonMaterial m) => new()
    {
        Id            = m.Id,
        Name          = m.Name,
        Slug          = m.Slug,
        PriceModifier = m.PriceModifier,
        IsActive      = m.IsActive,
        SortOrder     = m.SortOrder,
    };
}
