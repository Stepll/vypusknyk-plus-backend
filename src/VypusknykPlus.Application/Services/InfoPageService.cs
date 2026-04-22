using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Services;

public class InfoPageService(AppDbContext db) : IInfoPageService
{
    public async Task<List<InfoPageResponse>> GetAllAsync() =>
        await db.InfoPages
            .OrderBy(p => p.Order)
            .Select(p => ToResponse(p))
            .ToListAsync();

    public async Task<InfoPageResponse?> GetBySlugAsync(string slug) =>
        await db.InfoPages
            .Where(p => p.Slug == slug)
            .Select(p => ToResponse(p))
            .FirstOrDefaultAsync();

    public async Task<InfoPageResponse> UpdateBySlugAsync(string slug, UpdateInfoPageRequest request)
    {
        var page = await db.InfoPages.FirstOrDefaultAsync(p => p.Slug == slug)
            ?? throw new KeyNotFoundException($"Сторінку '{slug}' не знайдено");

        page.Title = request.Title.Trim();
        page.Content = request.Content.Trim();
        page.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return ToResponse(page);
    }

    private static InfoPageResponse ToResponse(InfoPage p) => new()
    {
        Id = p.Id,
        Slug = p.Slug,
        Title = p.Title,
        Content = p.Content,
        Order = p.Order,
        UpdatedAt = p.UpdatedAt,
    };
}
