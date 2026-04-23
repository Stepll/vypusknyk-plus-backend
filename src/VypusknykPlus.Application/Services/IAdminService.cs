using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Application.Services;

public interface IAdminService
{
    Task<PagedResponse<AdminOrderResponse>> GetOrdersAsync(int page, int pageSize, string? status);
    Task<AdminOrderResponse?> GetOrderAsync(long id);
    Task UpdateOrderStatusAsync(long id, string status);

    Task<PagedResponse<AdminProductResponse>> GetProductsAsync(int page, int pageSize);
    Task<AdminProductDetailResponse?> GetProductAsync(long id);
    Task<AdminProductDetailResponse> CreateProductAsync(SaveProductRequest request);
    Task<AdminProductDetailResponse> UpdateProductAsync(long id, SaveProductRequest request);
    Task DeleteProductAsync(long id);
    Task<AdminProductDetailResponse> UploadProductImageAsync(long productId, Stream stream, string contentType);
    Task<AdminProductDetailResponse> DeleteProductImageAsync(long productId, long imageId);
    Task<AdminProductDetailResponse> SetPreviewImageAsync(long productId, long imageId);

    Task<PagedResponse<AdminUserResponse>> GetUsersAsync(int page, int pageSize);
    Task<AdminUserDetailResponse?> GetUserAsync(long id);

    Task<PagedResponse<AdminSavedDesignResponse>> GetSavedDesignsAsync(int page, int pageSize);
    Task<AdminSavedDesignDetailResponse?> GetSavedDesignAsync(long id);
    Task<PagedResponse<AdminAdminResponse>> GetAdminsAsync(int page, int pageSize);
    Task<AdminAdminDetailResponse?> GetAdminDetailAsync(long id);
    Task<AdminAdminDetailResponse> CreateAdminAsync(CreateAdminRequest request);
    Task DeleteAdminAsync(long id);
    Task ChangeAdminPasswordAsync(long id, string newPassword);
    Task<AdminAdminDetailResponse> ChangeAdminRoleAsync(long id, long? roleId);
}
