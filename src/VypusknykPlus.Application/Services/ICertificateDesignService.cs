using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.DTOs.Designs;

namespace VypusknykPlus.Application.Services;

public interface ICertificateDesignService
{
    Task<CertificateDesignResponse> SaveAsync(long userId, SaveCertificateDesignRequest request);
    Task<List<CertificateDesignResponse>> GetUserDesignsAsync(long userId);
    Task<CertificateDesignResponse> UpdateAsync(long userId, long designId, SaveCertificateDesignRequest request);
    Task DeleteAsync(long userId, long designId);

    Task<PagedResponse<AdminCertificateDesignResponse>> GetAllAdminAsync(int page, int pageSize);
    Task<AdminCertificateDesignResponse?> GetAdminByIdAsync(long id);
    Task DeleteAdminAsync(long id);
    Task<List<AdminCertificateDesignResponse>> GetByUserIdAsync(long userId);
}
