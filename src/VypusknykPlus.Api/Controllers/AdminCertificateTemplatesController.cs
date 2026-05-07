using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/admin/certificate-templates")]
[Authorize(Roles = "Admin")]
public class AdminCertificateTemplatesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IImageService _imageService;

    public AdminCertificateTemplatesController(AppDbContext db, IImageService imageService)
    {
        _db = db;
        _imageService = imageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.CertificateTemplates
            .IgnoreQueryFilters().Where(x => !x.IsDeleted)
            .OrderBy(x => x.SortOrder).ThenBy(x => x.Id)
            .ToListAsync();
        return Ok(items.Select(Map));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var x = await _db.CertificateTemplates.IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        return x is null ? NotFound() : Ok(Map(x));
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaveCertificateTemplateRequest req)
    {
        var x = new CertificateTemplate
        {
            Name = req.Name, Slug = req.Slug,
            PriceModifier = req.PriceModifier, IsActive = req.IsActive, SortOrder = req.SortOrder,
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow,
        };
        _db.CertificateTemplates.Add(x);
        await _db.SaveChangesAsync();
        return Ok(Map(x));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, SaveCertificateTemplateRequest req)
    {
        var x = await _db.CertificateTemplates.IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        if (x is null) return NotFound();
        x.Name = req.Name; x.Slug = req.Slug;
        x.PriceModifier = req.PriceModifier; x.IsActive = req.IsActive; x.SortOrder = req.SortOrder;
        x.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(Map(x));
    }

    [HttpPut("{id:long}/layout")]
    public async Task<IActionResult> SaveLayout(long id, SaveCertificateTemplateLayoutRequest req)
    {
        var x = await _db.CertificateTemplates.IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        if (x is null) return NotFound();
        x.NativeOrientation = req.NativeOrientation;
        x.HasSecondSigner = req.HasSecondSigner;
        x.HasAdditionalText = req.HasAdditionalText;
        x.LayoutJson = req.LayoutJson;
        x.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(Map(x));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var x = await _db.CertificateTemplates.IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        if (x is null) return NotFound();
        x.IsDeleted = true; x.UpdatedAt = DateTime.UtcNow;
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

        var allowed = new[] { "image/png", "image/jpeg", "image/webp" };
        if (!allowed.Contains(image.ContentType))
            return BadRequest(new { message = "Only PNG, JPG or WebP files are allowed." });

        var x = await _db.CertificateTemplates.IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        if (x is null) return NotFound();

        if (!string.IsNullOrEmpty(x.ImageKey))
            await _imageService.DeleteAsync(x.ImageKey);

        var ext = image.ContentType switch
        {
            "image/png" => "png",
            "image/webp" => "webp",
            _ => "jpg",
        };
        var objectKey = $"certificate-templates/{id}.{ext}";
        await using var stream = image.OpenReadStream();
        await _imageService.UploadAsync(objectKey, stream, image.ContentType);

        x.ImageKey = objectKey;
        x.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(Map(x));
    }

    private CertificateTemplateResponse Map(CertificateTemplate x) => new()
    {
        Id = x.Id, Name = x.Name, Slug = x.Slug,
        ImageUrl = _imageService.GetPublicUrl(x.ImageKey),
        PriceModifier = x.PriceModifier, IsActive = x.IsActive, SortOrder = x.SortOrder,
        NativeOrientation = x.NativeOrientation, HasSecondSigner = x.HasSecondSigner,
        HasAdditionalText = x.HasAdditionalText, LayoutJson = x.LayoutJson,
    };
}
