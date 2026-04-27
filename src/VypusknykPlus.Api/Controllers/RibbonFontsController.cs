using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/ribbon-fonts")]
public class RibbonFontsController : ControllerBase
{
    private readonly AppDbContext _db;
    public RibbonFontsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var items = await _db.RibbonFonts
            .Where(f => f.IsActive)
            .OrderBy(f => f.SortOrder).ThenBy(f => f.Id)
            .Select(f => new RibbonFontResponse
            {
                Id         = f.Id,
                Name       = f.Name,
                Slug       = f.Slug,
                FontFamily = f.FontFamily,
                ImportUrl  = f.ImportUrl,
                IsActive   = f.IsActive,
                SortOrder  = f.SortOrder,
            })
            .ToListAsync();
        return Ok(items);
    }
}
