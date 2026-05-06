using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/admin/badge-images")]
[Authorize(Roles = "Admin")]
public class AdminBadgeImagesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IImageService _imageService;

    public AdminBadgeImagesController(AppDbContext db, IImageService imageService)
    {
        _db = db;
        _imageService = imageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rows = await _db.BadgeImages
            .IgnoreQueryFilters()
            .Where(i => !i.IsDeleted)
            .OrderBy(i => i.SortOrder).ThenBy(i => i.Id)
            .ToListAsync();
        return Ok(rows.Select(Map));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var i = await _db.BadgeImages.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        return i is null ? NotFound() : Ok(Map(i));
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaveBadgeImageRequest req)
    {
        var i = new BadgeImage
        {
            Name      = req.Name,
            IsActive  = req.IsActive,
            SortOrder = req.SortOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        _db.BadgeImages.Add(i);
        await _db.SaveChangesAsync();
        return Ok(Map(i));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, SaveBadgeImageRequest req)
    {
        var i = await _db.BadgeImages.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (i is null) return NotFound();

        i.Name      = req.Name;
        i.IsActive  = req.IsActive;
        i.SortOrder = req.SortOrder;
        i.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(Map(i));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var i = await _db.BadgeImages.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (i is null) return NotFound();
        i.IsDeleted = true;
        i.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:long}/image")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> UploadImage(long id, IFormFile image)
    {
        if (image is null || image.Length == 0)
            return BadRequest(new { message = "No file provided." });

        var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
        if (!allowed.Contains(image.ContentType))
            return BadRequest(new { message = "Only JPG, PNG, WebP files are allowed." });

        var i = await _db.BadgeImages.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (i is null) return NotFound();

        if (!string.IsNullOrEmpty(i.ImageKey))
            await _imageService.DeleteAsync(i.ImageKey);

        var ext = image.ContentType switch
        {
            "image/png"  => "png",
            "image/webp" => "webp",
            _            => "jpg",
        };
        var objectKey = $"badge-images/{id}.{ext}";
        await using var stream = image.OpenReadStream();
        await _imageService.UploadAsync(objectKey, stream, image.ContentType);

        i.ImageKey  = objectKey;
        i.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(Map(i));
    }

    private BadgeImageResponse Map(BadgeImage i) => new()
    {
        Id       = i.Id,
        Name     = i.Name,
        ImageUrl = _imageService.GetPublicUrl(i.ImageKey),
        IsActive = i.IsActive,
        SortOrder = i.SortOrder,
    };
}
