using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Application.Services;

public interface IInfoPageService
{
    Task<List<InfoPageResponse>> GetAllAsync();
    Task<InfoPageResponse?> GetBySlugAsync(string slug);
    Task<InfoPageResponse> UpdateBySlugAsync(string slug, UpdateInfoPageRequest request);
}
