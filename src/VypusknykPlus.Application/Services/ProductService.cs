using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Products;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;
    private readonly IImageService _imageService;
    private readonly ILogger<ProductService> _logger;

    public ProductService(AppDbContext db, IImageService imageService, ILogger<ProductService> logger)
    {
        _db = db;
        _imageService = imageService;
        _logger = logger;
    }

    public async Task<PagedResponse<ProductResponse>> GetAllAsync(ProductQueryParams query)
    {
        var q = _db.Products
            .Include(p => p.Category)
            .Include(p => p.Subcategory)
            .AsQueryable();

        // Filter by category
        if (long.TryParse(query.Category, out var categoryId))
        {
            q = q.Where(p => p.CategoryId == categoryId);
        }

        // Search by name and description
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower();
            q = q.Where(p => p.Name.ToLower().Contains(search) ||
                             p.Description.ToLower().Contains(search));
        }

        var total = await q.CountAsync();

        // Sort
        q = query.Sort?.ToLower() switch
        {
            "price-asc" => q.OrderBy(p => p.Price),
            "price-desc" => q.OrderByDescending(p => p.Price),
            "name-asc" => q.OrderBy(p => p.Name),
            _ => q.OrderByDescending(p => p.Popular).ThenByDescending(p => p.CreatedAt)
        };

        // Pagination
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 50);

        // Materialize first, then project — EF Core cannot translate instance method calls to SQL
        var products = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        _logger.LogInformation("Products query: category={Category}, search={Search}, sort={Sort}, page={Page}, total={Total}",
            query.Category, query.Search, query.Sort, page, total);

        return new PagedResponse<ProductResponse>
        {
            Items = [.. products.Select(MapToResponse)],
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ProductResponse?> GetByIdAsync(long id)
    {
        var product = await _db.Products
            .Include(p => p.Category)
            .Include(p => p.Subcategory)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
            return null;

        return MapToResponse(product);
    }

    public async Task<ProductResponse> UploadImageAsync(long productId, Stream imageStream, string contentType)
    {
        var product = await _db.Products
            .Include(p => p.Category)
            .Include(p => p.Subcategory)
            .FirstOrDefaultAsync(p => p.Id == productId)
            ?? throw new KeyNotFoundException($"Product {productId} not found");

        // Delete old image if replacing with a different format
        if (!string.IsNullOrEmpty(product.ImageKey))
            await _imageService.DeleteAsync(product.ImageKey);

        var key = await _imageService.UploadProductImageAsync(productId, imageStream, contentType);
        product.ImageKey = key;
        product.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        _logger.LogInformation("Image updated for product {ProductId}", productId);
        return MapToResponse(product);
    }

    public async Task<ProductResponse> DeleteImageAsync(long productId)
    {
        var product = await _db.Products
            .Include(p => p.Category)
            .Include(p => p.Subcategory)
            .FirstOrDefaultAsync(p => p.Id == productId)
            ?? throw new KeyNotFoundException($"Product {productId} not found");

        if (!string.IsNullOrEmpty(product.ImageKey))
        {
            await _imageService.DeleteAsync(product.ImageKey);
            product.ImageKey = null;
            product.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        return MapToResponse(product);
    }

    private ProductResponse MapToResponse(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        CategoryId = p.CategoryId,
        CategoryName = p.Category.Name,
        SubcategoryId = p.SubcategoryId,
        SubcategoryName = p.Subcategory?.Name,
        Color = p.Color,
        Price = p.Price,
        MinOrder = p.MinOrder,
        Popular = p.Popular,
        IsNew = p.IsNew,
        Description = p.Description,
        Tags = p.Tags,
        ImageUrl = _imageService.GetPublicUrl(p.ImageKey)
    };
}
