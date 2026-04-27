using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/ribbon-print-colors")]
public class RibbonPrintColorsController : ControllerBase
{
    private readonly AppDbContext _db;
    public RibbonPrintColorsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var items = await _db.RibbonPrintColors
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder).ThenBy(c => c.Id)
            .Select(c => new RibbonPrintColorResponse
            {
                Id            = c.Id,
                Name          = c.Name,
                Slug          = c.Slug,
                Hex           = c.Hex,
                PriceModifier = c.PriceModifier,
                IsActive      = c.IsActive,
                SortOrder     = c.SortOrder,
            })
            .ToListAsync();
        return Ok(items);
    }
}
