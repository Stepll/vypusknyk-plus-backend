using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/admin/certificate-paper-types")]
[Authorize(Roles = "Admin")]
public class AdminCertificatePaperTypesController : ControllerBase
{
    private readonly AppDbContext _db;
    public AdminCertificatePaperTypesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.CertificatePaperTypes
            .IgnoreQueryFilters().Where(x => !x.IsDeleted)
            .OrderBy(x => x.SortOrder).ThenBy(x => x.Id)
            .Select(x => Map(x)).ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var x = await _db.CertificatePaperTypes.IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        return x is null ? NotFound() : Ok(Map(x));
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaveCertificatePaperTypeRequest req)
    {
        var x = new CertificatePaperType
        {
            Name = req.Name, Slug = req.Slug,
            PriceModifier = req.PriceModifier, IsActive = req.IsActive, SortOrder = req.SortOrder,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow,
        };
        _db.CertificatePaperTypes.Add(x);
        await _db.SaveChangesAsync();
        return Ok(Map(x));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, SaveCertificatePaperTypeRequest req)
    {
        var x = await _db.CertificatePaperTypes.IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (x is null) return NotFound();
        x.Name = req.Name; x.Slug = req.Slug;
        x.PriceModifier = req.PriceModifier; x.IsActive = req.IsActive; x.SortOrder = req.SortOrder;
        x.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(Map(x));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var x = await _db.CertificatePaperTypes.IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        if (x is null) return NotFound();
        x.IsDeleted = true; x.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static CertificatePaperTypeResponse Map(CertificatePaperType x) => new()
    {
        Id = x.Id, Name = x.Name, Slug = x.Slug,
        PriceModifier = x.PriceModifier, IsActive = x.IsActive, SortOrder = x.SortOrder,
    };
}
