using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/admin/badge-text-sizes")]
[Authorize(Roles = "Admin")]
public class AdminBadgeTextSizesController : ControllerBase
{
    private readonly AppDbContext _db;
    public AdminBadgeTextSizesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.BadgeTextSizes
            .IgnoreQueryFilters().Where(x => !x.IsDeleted)
            .OrderBy(x => x.SortOrder).ThenBy(x => x.Id)
            .Select(x => Map(x)).ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var x = await _db.BadgeTextSizes.IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        return x is null ? NotFound() : Ok(Map(x));
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaveBadgeTextSizeRequest req)
    {
        var x = new BadgeTextSize
        {
            Label = req.Label, Value = req.Value,
            IsActive = req.IsActive, SortOrder = req.SortOrder,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow,
        };
        _db.BadgeTextSizes.Add(x);
        await _db.SaveChangesAsync();
        return Ok(Map(x));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, SaveBadgeTextSizeRequest req)
    {
        var x = await _db.BadgeTextSizes.IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        if (x is null) return NotFound();
        x.Label = req.Label; x.Value = req.Value;
        x.IsActive = req.IsActive; x.SortOrder = req.SortOrder;
        x.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(Map(x));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var x = await _db.BadgeTextSizes.IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        if (x is null) return NotFound();
        x.IsDeleted = true; x.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static BadgeTextSizeResponse Map(BadgeTextSize x) => new()
    {
        Id = x.Id, Label = x.Label, Value = x.Value,
        IsActive = x.IsActive, SortOrder = x.SortOrder,
    };
}
