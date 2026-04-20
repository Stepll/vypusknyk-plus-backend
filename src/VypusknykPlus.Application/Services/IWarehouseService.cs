using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Application.Services;

public interface IWarehouseService
{
    Task<List<StockCategoryResponse>> GetCategoriesAsync();
    Task<List<StockSubcategoryResponse>> GetSubcategoriesAsync(long? categoryId);
    Task<WarehouseStatsResponse> GetStatsAsync();
    Task<PagedResponse<StockProductSummary>> GetProductsAsync(WarehouseProductsQuery query);
    Task<StockProductDetail?> GetProductDetailAsync(long id);
    Task<StockTransactionResponse> AddTransactionAsync(CreateStockTransactionRequest request);
    Task<StockProductSummary> CreateProductAsync(CreateStockProductRequest request);
}
