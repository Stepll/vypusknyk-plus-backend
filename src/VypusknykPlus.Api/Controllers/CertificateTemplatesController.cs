using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/certificate-templates")]
public class CertificateTemplatesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IImageService _imageService;

    public CertificateTemplatesController(AppDbContext db, IImageService imageService)
    {
        _db = db;
        _imageService = imageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var items = await _db.CertificateTemplates
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder).ThenBy(x => x.Id)
            .ToListAsync();

        return Ok(items.Select(x => new CertificateTemplateResponse
        {
            Id = x.Id, Name = x.Name, Slug = x.Slug,
            ImageUrl = _imageService.GetPublicUrl(x.ImageKey),
            PriceModifier = x.PriceModifier, IsActive = x.IsActive, SortOrder = x.SortOrder,
            NativeOrientation = x.NativeOrientation, HasSecondSigner = x.HasSecondSigner,
            HasAdditionalText = x.HasAdditionalText, LayoutJson = x.LayoutJson,
        }));
    }
}
