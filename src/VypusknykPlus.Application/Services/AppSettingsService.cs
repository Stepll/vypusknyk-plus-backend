using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.AppSettings;

namespace VypusknykPlus.Application.Services;

public class AppSettingsService(AppDbContext db) : IAppSettingsService
{
    public async Task<List<AppSettingResponse>> GetAllAsync()
    {
        var settings = await db.AppSettings.OrderBy(s => s.Group).ThenBy(s => s.Key).ToListAsync();
        return settings.Select(s => new AppSettingResponse(
            s.Key, s.Value, s.Type, s.Group, s.Label, s.Description, s.UpdatedAt
        )).ToList();
    }

    public async Task<Dictionary<string, string>> GetPublicAsync()
    {
        var settings = await db.AppSettings.ToListAsync();
        return settings.ToDictionary(s => s.Key, s => s.Value);
    }

    public async Task UpdateManyAsync(List<UpdateAppSettingRequest> updates)
    {
        var keys = updates.Select(u => u.Key).ToList();
        var existing = await db.AppSettings.Where(s => keys.Contains(s.Key)).ToListAsync();

        var now = DateTime.UtcNow;
        foreach (var update in updates)
        {
            var setting = existing.FirstOrDefault(s => s.Key == update.Key);
            if (setting is null) continue;
            setting.Value = update.Value;
            setting.UpdatedAt = now;
        }

        await db.SaveChangesAsync();
    }
}
