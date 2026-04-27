using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/ribbon-colors")]
public class RibbonColorsController : ControllerBase
{
    private readonly AppDbContext _db;
    public RibbonColorsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var items = await _db.RibbonColors
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Id)
            .Select(c => new RibbonColorResponse
            {
                Id            = c.Id,
                Name          = c.Name,
                Slug          = c.Slug,
                Hex           = c.Hex,
                SecondaryHex  = c.SecondaryHex,
                PriceModifier = c.PriceModifier,
                IsActive      = c.IsActive,
                SortOrder     = c.SortOrder,
            })
            .ToListAsync();
        return Ok(items);
    }
}
