using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Application.Services;

public interface IAdminRoleService
{
    Task<List<RoleResponse>> GetRolesAsync();
    Task<RoleResponse?> GetRoleAsync(long id);
    Task<RoleResponse> CreateRoleAsync(CreateRoleRequest request);
    Task<RoleResponse> UpdateRoleAsync(long id, UpdateRoleRequest request);
    Task DeleteRoleAsync(long id);
}
