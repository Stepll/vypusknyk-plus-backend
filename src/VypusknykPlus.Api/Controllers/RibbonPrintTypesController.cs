using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/ribbon-print-types")]
public class RibbonPrintTypesController : ControllerBase
{
    private readonly AppDbContext _db;
    public RibbonPrintTypesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var items = await _db.RibbonPrintTypes
            .Where(p => p.IsActive)
            .OrderBy(p => p.SortOrder).ThenBy(p => p.Id)
            .Select(p => new RibbonPrintTypeResponse
            {
                Id            = p.Id,
                Name          = p.Name,
                Slug          = p.Slug,
                PriceModifier = p.PriceModifier,
                IsActive      = p.IsActive,
                SortOrder     = p.SortOrder,
            })
            .ToListAsync();
        return Ok(items);
    }
}
