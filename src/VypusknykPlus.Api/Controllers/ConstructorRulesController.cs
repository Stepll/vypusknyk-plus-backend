using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/constructor/rules")]
public class ConstructorRulesController : ControllerBase
{
    private readonly AppDbContext _db;
    public ConstructorRulesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var incompatibilities = await _db.ConstructorIncompatibilities
            .Include(r => r.Targets)
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.Id)
            .ToListAsync();

        var forcedTexts = await _db.ConstructorForcedTexts
            .Include(r => r.Values)
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.Id)
            .ToListAsync();

        return Ok(new ConstructorRulesResponse
        {
            Incompatibilities = incompatibilities.Select(r => new ConstructorIncompatibilityResponse
            {
                Id        = r.Id,
                TypeA     = r.TypeA,
                SlugA     = r.SlugA,
                TypeB     = r.TypeB,
                IsWarning = r.IsWarning,
                Message   = r.Message,
                SlugsB    = r.Targets.Select(t => t.SlugB).ToList()
            }).ToList(),

            ForcedTexts = forcedTexts.Select(r => new ConstructorForcedTextResponse
            {
                Id          = r.Id,
                TriggerType = r.TriggerType,
                TriggerSlug = r.TriggerSlug,
                TargetField = r.TargetField,
                Message     = r.Message,
                Values      = r.Values.Select(v => v.Value).ToList()
            }).ToList()
        });
    }
}
