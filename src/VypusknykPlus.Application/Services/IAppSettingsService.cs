using VypusknykPlus.Application.DTOs.AppSettings;

namespace VypusknykPlus.Application.Services;

public interface IAppSettingsService
{
    Task<List<AppSettingResponse>> GetAllAsync();
    Task<Dictionary<string, string>> GetPublicAsync();
    Task UpdateManyAsync(List<UpdateAppSettingRequest> updates);
}
