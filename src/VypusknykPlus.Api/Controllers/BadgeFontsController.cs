using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/badge-fonts")]
public class BadgeFontsController : ControllerBase
{
    private readonly AppDbContext _db;
    public BadgeFontsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var items = await _db.BadgeFonts
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder).ThenBy(x => x.Id)
            .Select(x => new BadgeFontResponse
            {
                Id = x.Id, Name = x.Name, Slug = x.Slug, FontFamily = x.FontFamily,
                PriceModifier = x.PriceModifier, IsActive = x.IsActive, SortOrder = x.SortOrder,
            })
            .ToListAsync();
        return Ok(items);
    }
}
