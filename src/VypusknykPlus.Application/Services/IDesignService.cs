using VypusknykPlus.Application.DTOs.Designs;

namespace VypusknykPlus.Application.Services;

public interface IDesignService
{
    Task<DesignResponse> SaveAsync(Guid userId, SaveDesignRequest request);
    Task<List<DesignResponse>> GetUserDesignsAsync(Guid userId);
    Task<DesignResponse> UpdateAsync(Guid userId, Guid designId, SaveDesignRequest request);
    Task DeleteAsync(Guid userId, Guid designId);
}
