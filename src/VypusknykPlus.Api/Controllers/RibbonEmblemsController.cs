using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/ribbon-emblems")]
public class RibbonEmblemsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IImageService _imageService;

    public RibbonEmblemsController(AppDbContext db, IImageService imageService)
    {
        _db = db;
        _imageService = imageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var rows = await _db.RibbonEmblems
            .Where(e => e.IsActive)
            .OrderBy(e => e.SortOrder).ThenBy(e => e.Id)
            .ToListAsync();
        return Ok(rows.Select(e => new RibbonEmblemResponse
        {
            Id        = e.Id,
            Name      = e.Name,
            Slug      = e.Slug,
            SvgUrl    = _imageService.GetPublicUrl(e.SvgKey),
            IsActive  = e.IsActive,
            SortOrder = e.SortOrder,
        }));
    }
}
