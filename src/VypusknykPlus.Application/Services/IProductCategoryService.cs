using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Application.Services;

public interface IProductCategoryService
{
    Task<List<ProductCategoryResponse>> GetAllAsync();
    Task<ProductCategoryResponse> CreateCategoryAsync(SaveProductCategoryRequest request);
    Task<ProductCategoryResponse> UpdateCategoryAsync(long id, SaveProductCategoryRequest request);
    Task DeleteCategoryAsync(long id);

    Task<ProductSubcategoryResponse> CreateSubcategoryAsync(long categoryId, SaveProductSubcategoryRequest request);
    Task<ProductSubcategoryResponse> UpdateSubcategoryAsync(long id, SaveProductSubcategoryRequest request);
    Task DeleteSubcategoryAsync(long id);
}
