using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.DTOs.Designs;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Services;

public class CertificateDesignService : ICertificateDesignService
{
    private readonly AppDbContext _db;

    public CertificateDesignService(AppDbContext db) => _db = db;

    public async Task<CertificateDesignResponse> SaveAsync(long userId, SaveCertificateDesignRequest request)
    {
        var design = new SavedCertificateDesign
        {
            DesignName = request.DesignName,
            SavedAt = DateTime.UtcNow,
            StateJson = request.State.GetRawText(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _db.SavedCertificateDesigns.Add(design);
        await _db.SaveChangesAsync();
        return MapResponse(design);
    }

    public async Task<List<CertificateDesignResponse>> GetUserDesignsAsync(long userId)
    {
        var designs = await _db.SavedCertificateDesigns
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.SavedAt)
            .ToListAsync();

        return designs.Select(MapResponse).ToList();
    }

    public async Task<CertificateDesignResponse> UpdateAsync(long userId, long designId, SaveCertificateDesignRequest request)
    {
        var design = await _db.SavedCertificateDesigns
            .FirstOrDefaultAsync(d => d.Id == designId && d.UserId == userId)
            ?? throw new KeyNotFoundException("Дизайн не знайдено");

        design.DesignName = request.DesignName;
        design.StateJson = request.State.GetRawText();
        design.SavedAt = DateTime.UtcNow;
        design.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return MapResponse(design);
    }

    public async Task DeleteAsync(long userId, long designId)
    {
        var design = await _db.SavedCertificateDesigns
            .FirstOrDefaultAsync(d => d.Id == designId && d.UserId == userId)
            ?? throw new KeyNotFoundException("Дизайн не знайдено");

        design.IsDeleted = true;
        design.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<PagedResponse<AdminCertificateDesignResponse>> GetAllAdminAsync(int page, int pageSize)
    {
        var query = _db.SavedCertificateDesigns
            .Include(d => d.User)
            .OrderByDescending(d => d.SavedAt);

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<AdminCertificateDesignResponse>
        {
            Items = items.Select(MapAdminResponse).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task<AdminCertificateDesignResponse?> GetAdminByIdAsync(long id)
    {
        var design = await _db.SavedCertificateDesigns
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == id);

        return design is null ? null : MapAdminResponse(design);
    }

    public async Task DeleteAdminAsync(long id)
    {
        var design = await _db.SavedCertificateDesigns.FindAsync(id)
            ?? throw new KeyNotFoundException("Дизайн не знайдено");

        design.IsDeleted = true;
        design.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<List<AdminCertificateDesignResponse>> GetByUserIdAsync(long userId)
    {
        var designs = await _db.SavedCertificateDesigns
            .Include(d => d.User)
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.SavedAt)
            .ToListAsync();

        return designs.Select(MapAdminResponse).ToList();
    }

    private static CertificateDesignResponse MapResponse(SavedCertificateDesign d)
    {
        JsonElement state;
        try { state = JsonSerializer.Deserialize<JsonElement>(d.StateJson); }
        catch { state = JsonSerializer.Deserialize<JsonElement>("{}"); }

        return new CertificateDesignResponse
        {
            Id = d.Id,
            DesignName = d.DesignName,
            SavedAt = d.SavedAt,
            State = state,
        };
    }

    private static AdminCertificateDesignResponse MapAdminResponse(SavedCertificateDesign d)
    {
        JsonElement state;
        try { state = JsonSerializer.Deserialize<JsonElement>(d.StateJson); }
        catch { state = JsonSerializer.Deserialize<JsonElement>("{}"); }

        return new AdminCertificateDesignResponse
        {
            Id = d.Id,
            DesignName = d.DesignName,
            SavedAt = d.SavedAt,
            UserId = d.UserId,
            UserFullName = d.User.FullName,
            UserEmail = d.User.Email,
            State = state,
        };
    }
}
