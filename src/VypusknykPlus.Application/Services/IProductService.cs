using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Products;

namespace VypusknykPlus.Application.Services;

public interface IProductService
{
    Task<PagedResponse<ProductResponse>> GetAllAsync(ProductQueryParams query);
    Task<ProductResponse?> GetByIdAsync(long id);
    Task<ProductResponse> UploadImageAsync(long productId, Stream imageStream, string contentType);
    Task<ProductResponse> DeleteImageAsync(long productId);
}
