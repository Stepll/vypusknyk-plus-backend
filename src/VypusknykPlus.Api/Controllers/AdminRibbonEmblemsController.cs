using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/admin/ribbon-emblems")]
[Authorize(Roles = "Admin")]
public class AdminRibbonEmblemsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IImageService _imageService;

    public AdminRibbonEmblemsController(AppDbContext db, IImageService imageService)
    {
        _db = db;
        _imageService = imageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rows = await _db.RibbonEmblems
            .IgnoreQueryFilters()
            .Where(e => !e.IsDeleted)
            .OrderBy(e => e.SortOrder).ThenBy(e => e.Id)
            .ToListAsync();
        return Ok(rows.Select(Map));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var e = await _db.RibbonEmblems.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        return e is null ? NotFound() : Ok(Map(e));
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaveRibbonEmblemRequest req)
    {
        var e = new RibbonEmblem
        {
            Name      = req.Name,
            Slug      = req.Slug,
            IsActive  = req.IsActive,
            SortOrder = req.SortOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        _db.RibbonEmblems.Add(e);
        await _db.SaveChangesAsync();
        return Ok(Map(e));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, SaveRibbonEmblemRequest req)
    {
        var e = await _db.RibbonEmblems.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (e is null) return NotFound();

        e.Name      = req.Name;
        e.Slug      = req.Slug;
        e.IsActive  = req.IsActive;
        e.SortOrder = req.SortOrder;
        e.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(Map(e));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var e = await _db.RibbonEmblems.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (e is null) return NotFound();
        e.IsDeleted = true;
        e.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:long}/svg/left")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(2 * 1024 * 1024)]
    public Task<IActionResult> UploadSvgLeft(long id, IFormFile svg) =>
        UploadSvgSide(id, svg, "left");

    [HttpPost("{id:long}/svg/right")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(2 * 1024 * 1024)]
    public Task<IActionResult> UploadSvgRight(long id, IFormFile svg) =>
        UploadSvgSide(id, svg, "right");

    private async Task<IActionResult> UploadSvgSide(long id, IFormFile svg, string side)
    {
        if (svg is null || svg.Length == 0)
            return BadRequest(new { message = "No file provided." });
        if (svg.ContentType != "image/svg+xml" && !svg.FileName.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { message = "Only SVG files are allowed." });

        var e = await _db.RibbonEmblems.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (e is null) return NotFound();

        var oldKey = side == "left" ? e.SvgKeyLeft : e.SvgKeyRight;
        if (!string.IsNullOrEmpty(oldKey))
            await _imageService.DeleteAsync(oldKey);

        var objectKey = $"emblems/{id}-{side}.svg";
        await using var stream = svg.OpenReadStream();
        await _imageService.UploadAsync(objectKey, stream, "image/svg+xml");

        if (side == "left") e.SvgKeyLeft  = objectKey;
        else                e.SvgKeyRight = objectKey;
        e.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(Map(e));
    }

    private RibbonEmblemResponse Map(RibbonEmblem e) => new()
    {
        Id          = e.Id,
        Name        = e.Name,
        Slug        = e.Slug,
        SvgUrlLeft  = _imageService.GetPublicUrl(e.SvgKeyLeft),
        SvgUrlRight = _imageService.GetPublicUrl(e.SvgKeyRight),
        IsActive    = e.IsActive,
        SortOrder   = e.SortOrder,
    };
}
