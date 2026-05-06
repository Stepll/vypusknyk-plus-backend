using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/badge-sizes")]
public class BadgeSizesController : ControllerBase
{
    private readonly AppDbContext _db;
    public BadgeSizesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var items = await _db.BadgeSizes
            .Where(s => s.IsActive)
            .OrderBy(s => s.SortOrder).ThenBy(s => s.Id)
            .Select(s => new BadgeSizeResponse
            {
                Id            = s.Id,
                Name          = s.Name,
                Diameter      = s.Diameter,
                PriceModifier = s.PriceModifier,
                IsActive      = s.IsActive,
                SortOrder     = s.SortOrder,
            })
            .ToListAsync();
        return Ok(items);
    }
}
