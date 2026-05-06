using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/badge-text-sizes")]
public class BadgeTextSizesController : ControllerBase
{
    private readonly AppDbContext _db;
    public BadgeTextSizesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetActive()
    {
        var items = await _db.BadgeTextSizes
            .Where(x => x.IsActive)
            .OrderBy(x => x.SortOrder).ThenBy(x => x.Id)
            .Select(x => new BadgeTextSizeResponse
            {
                Id = x.Id, Label = x.Label, Value = x.Value,
                IsActive = x.IsActive, SortOrder = x.SortOrder,
            })
            .ToListAsync();
        return Ok(items);
    }
}
