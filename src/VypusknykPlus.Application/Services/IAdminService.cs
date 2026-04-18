using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Application.Services;

public interface IAdminService
{
    Task<PagedResponse<AdminOrderResponse>> GetOrdersAsync(int page, int pageSize, string? status);
    Task<AdminOrderResponse?> GetOrderAsync(Guid id);
    Task UpdateOrderStatusAsync(Guid id, string status);

    Task<PagedResponse<AdminProductResponse>> GetProductsAsync(int page, int pageSize);
    Task DeleteProductAsync(int id);

    Task<PagedResponse<AdminUserResponse>> GetUsersAsync(int page, int pageSize);
    Task<AdminUserResponse?> GetUserAsync(Guid id);
}
