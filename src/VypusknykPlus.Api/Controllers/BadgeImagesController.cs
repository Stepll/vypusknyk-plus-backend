using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/badge-images")]
public class BadgeImagesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IImageService _imageService;

    public BadgeImagesController(AppDbContext db, IImageService imageService)
    {
        _db = db;
        _imageService = imageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var rows = await _db.BadgeImages
            .Where(i => i.IsActive)
            .OrderBy(i => i.SortOrder).ThenBy(i => i.Id)
            .ToListAsync();
        return Ok(rows.Select(i => new BadgeImageResponse
        {
            Id       = i.Id,
            Name     = i.Name,
            ImageUrl = _imageService.GetPublicUrl(i.ImageKey),
            IsActive = i.IsActive,
            SortOrder = i.SortOrder,
        }));
    }
}
