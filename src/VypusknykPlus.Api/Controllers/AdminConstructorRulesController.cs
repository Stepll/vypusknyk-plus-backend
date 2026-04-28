using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/admin/constructor-rules")]
[Authorize(Roles = "Admin")]
public class AdminConstructorRulesController : ControllerBase
{
    private readonly AppDbContext _db;
    public AdminConstructorRulesController(AppDbContext db) => _db = db;

    // ── Incompatibilities ────────────────────────────────────────────────────

    [HttpGet("incompatibilities")]
    public async Task<IActionResult> GetIncompatibilities()
    {
        var items = await _db.ConstructorIncompatibilities
            .Include(r => r.Targets)
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.Id)
            .ToListAsync();

        return Ok(items.Select(MapIncompat));
    }

    [HttpGet("incompatibilities/{id:long}")]
    public async Task<IActionResult> GetIncompatibility(long id)
    {
        var rule = await _db.ConstructorIncompatibilities
            .Include(r => r.Targets)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        return rule is null ? NotFound() : Ok(MapIncompat(rule));
    }

    [HttpPost("incompatibilities")]
    public async Task<IActionResult> CreateIncompatibility(SaveConstructorIncompatibilityRequest req)
    {
        var rule = new ConstructorIncompatibility
        {
            TypeA      = req.TypeA,
            SlugA      = req.SlugA,
            TypeB      = req.TypeB,
            IsWarning  = req.IsWarning,
            Message    = req.Message,
            CreatedAt  = DateTime.UtcNow,
            UpdatedAt  = DateTime.UtcNow,
            Targets    = req.SlugsB.Select(s => new ConstructorIncompatibilityTarget { SlugB = s }).ToList()
        };

        _db.ConstructorIncompatibilities.Add(rule);
        await _db.SaveChangesAsync();
        return Ok(MapIncompat(rule));
    }

    [HttpPut("incompatibilities/{id:long}")]
    public async Task<IActionResult> UpdateIncompatibility(long id, SaveConstructorIncompatibilityRequest req)
    {
        var rule = await _db.ConstructorIncompatibilities
            .Include(r => r.Targets)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (rule is null) return NotFound();

        rule.TypeA     = req.TypeA;
        rule.SlugA     = req.SlugA;
        rule.TypeB     = req.TypeB;
        rule.IsWarning = req.IsWarning;
        rule.Message   = req.Message;
        rule.UpdatedAt = DateTime.UtcNow;

        _db.ConstructorIncompatibilityTargets.RemoveRange(rule.Targets);
        rule.Targets = req.SlugsB.Select(s => new ConstructorIncompatibilityTarget { SlugB = s }).ToList();

        await _db.SaveChangesAsync();
        return Ok(MapIncompat(rule));
    }

    [HttpDelete("incompatibilities/{id:long}")]
    public async Task<IActionResult> DeleteIncompatibility(long id)
    {
        var rule = await _db.ConstructorIncompatibilities
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (rule is null) return NotFound();

        rule.IsDeleted = true;
        rule.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ── Forced Texts ─────────────────────────────────────────────────────────

    [HttpGet("forced-texts")]
    public async Task<IActionResult> GetForcedTexts()
    {
        var items = await _db.ConstructorForcedTexts
            .Include(r => r.Values)
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.Id)
            .ToListAsync();

        return Ok(items.Select(MapForcedText));
    }

    [HttpGet("forced-texts/{id:long}")]
    public async Task<IActionResult> GetForcedText(long id)
    {
        var rule = await _db.ConstructorForcedTexts
            .Include(r => r.Values)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        return rule is null ? NotFound() : Ok(MapForcedText(rule));
    }

    [HttpPost("forced-texts")]
    public async Task<IActionResult> CreateForcedText(SaveConstructorForcedTextRequest req)
    {
        var rule = new ConstructorForcedText
        {
            TriggerType  = req.TriggerType,
            TriggerSlug  = req.TriggerSlug,
            TargetField  = req.TargetField,
            Message      = req.Message,
            CreatedAt    = DateTime.UtcNow,
            UpdatedAt    = DateTime.UtcNow,
            Values       = req.Values.Select(v => new ConstructorForcedTextValue { Value = v }).ToList()
        };

        _db.ConstructorForcedTexts.Add(rule);
        await _db.SaveChangesAsync();
        return Ok(MapForcedText(rule));
    }

    [HttpPut("forced-texts/{id:long}")]
    public async Task<IActionResult> UpdateForcedText(long id, SaveConstructorForcedTextRequest req)
    {
        var rule = await _db.ConstructorForcedTexts
            .Include(r => r.Values)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (rule is null) return NotFound();

        rule.TriggerType  = req.TriggerType;
        rule.TriggerSlug  = req.TriggerSlug;
        rule.TargetField  = req.TargetField;
        rule.Message      = req.Message;
        rule.UpdatedAt    = DateTime.UtcNow;

        _db.ConstructorForcedTextValues.RemoveRange(rule.Values);
        rule.Values = req.Values.Select(v => new ConstructorForcedTextValue { Value = v }).ToList();

        await _db.SaveChangesAsync();
        return Ok(MapForcedText(rule));
    }

    [HttpDelete("forced-texts/{id:long}")]
    public async Task<IActionResult> DeleteForcedText(long id)
    {
        var rule = await _db.ConstructorForcedTexts
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

        if (rule is null) return NotFound();

        rule.IsDeleted = true;
        rule.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // ── Mappers ───────────────────────────────────────────────────────────────

    private static ConstructorIncompatibilityResponse MapIncompat(ConstructorIncompatibility r) => new()
    {
        Id        = r.Id,
        TypeA     = r.TypeA,
        SlugA     = r.SlugA,
        TypeB     = r.TypeB,
        IsWarning = r.IsWarning,
        Message   = r.Message,
        SlugsB    = r.Targets.Select(t => t.SlugB).ToList()
    };

    private static ConstructorForcedTextResponse MapForcedText(ConstructorForcedText r) => new()
    {
        Id          = r.Id,
        TriggerType = r.TriggerType,
        TriggerSlug = r.TriggerSlug,
        TargetField = r.TargetField,
        Message     = r.Message,
        Values      = r.Values.Select(v => v.Value).ToList()
    };
}
