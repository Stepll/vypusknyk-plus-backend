using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/ribbon-materials")]
public class RibbonMaterialsController : ControllerBase
{
    private readonly AppDbContext _db;
    public RibbonMaterialsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var items = await _db.RibbonMaterials
            .Where(m => m.IsActive)
            .OrderBy(m => m.SortOrder).ThenBy(m => m.Id)
            .Select(m => new RibbonMaterialResponse
            {
                Id            = m.Id,
                Name          = m.Name,
                Slug          = m.Slug,
                PriceModifier = m.PriceModifier,
                IsActive      = m.IsActive,
                SortOrder     = m.SortOrder,
            })
            .ToListAsync();
        return Ok(items);
    }
}
