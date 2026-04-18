using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Designs;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Services;

public class DesignService : IDesignService
{
    private readonly AppDbContext _db;
    private readonly ILogger<DesignService> _logger;

    public DesignService(AppDbContext db, ILogger<DesignService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<DesignResponse> SaveAsync(long userId, SaveDesignRequest request)
    {
        var design = new SavedDesign
        {
            Id = Guid.NewGuid(),
            DesignName = request.DesignName,
            SavedAt = DateTime.UtcNow,
            State = request.State,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.SavedDesigns.Add(design);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Design {DesignId} saved for user {UserId}", design.Id, userId);

        return MapToResponse(design);
    }

    public async Task<List<DesignResponse>> GetUserDesignsAsync(long userId)
    {
        return await _db.SavedDesigns
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.SavedAt)
            .Select(d => MapToResponse(d))
            .ToListAsync();
    }

    public async Task<DesignResponse> UpdateAsync(long userId, Guid designId, SaveDesignRequest request)
    {
        var design = await _db.SavedDesigns
            .FirstOrDefaultAsync(d => d.Id == designId && d.UserId == userId);

        if (design is null)
            throw new KeyNotFoundException("Дизайн не знайдено");

        design.DesignName = request.DesignName;
        design.State = request.State;
        design.SavedAt = DateTime.UtcNow;
        design.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        _logger.LogInformation("Design {DesignId} updated by user {UserId}", designId, userId);

        return MapToResponse(design);
    }

    public async Task DeleteAsync(long userId, Guid designId)
    {
        var design = await _db.SavedDesigns
            .FirstOrDefaultAsync(d => d.Id == designId && d.UserId == userId);

        if (design is null)
            throw new KeyNotFoundException("Дизайн не знайдено");

        design.IsDeleted = true;
        design.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        _logger.LogInformation("Design {DesignId} deleted by user {UserId}", designId, userId);
    }

    private static DesignResponse MapToResponse(SavedDesign d) => new()
    {
        Id = d.Id,
        DesignName = d.DesignName,
        SavedAt = d.SavedAt,
        State = d.State
    };
}
