using VypusknykPlus.Application.DTOs.Designs;

namespace VypusknykPlus.Application.Services;

public interface IDesignService
{
    Task<DesignResponse> SaveAsync(long userId, SaveDesignRequest request);
    Task<List<DesignResponse>> GetUserDesignsAsync(long userId);
    Task<DesignResponse> UpdateAsync(long userId, Guid designId, SaveDesignRequest request);
    Task DeleteAsync(long userId, Guid designId);
}
