using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.DTOs.Designs;

namespace VypusknykPlus.Application.Services;

public interface IBadgeDesignService
{
    Task<BadgeDesignResponse> SaveAsync(long userId, SaveBadgeDesignRequest request);
    Task<List<BadgeDesignResponse>> GetUserDesignsAsync(long userId);
    Task<BadgeDesignResponse> UpdateAsync(long userId, long designId, SaveBadgeDesignRequest request);
    Task DeleteAsync(long userId, long designId);

    Task<PagedResponse<AdminBadgeDesignResponse>> GetAllAdminAsync(int page, int pageSize);
    Task<AdminBadgeDesignResponse?> GetAdminByIdAsync(long id);
    Task DeleteAdminAsync(long id);
    Task<List<AdminBadgeDesignResponse>> GetByUserIdAsync(long userId);
}
