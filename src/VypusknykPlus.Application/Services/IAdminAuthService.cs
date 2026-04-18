using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Application.Services;

public interface IAdminAuthService
{
    Task<AdminAuthResponse> LoginAsync(AdminLoginRequest request);
}
