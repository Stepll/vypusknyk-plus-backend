using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Services;

public class ProductCategoryService : IProductCategoryService
{
    private readonly AppDbContext _db;

    public ProductCategoryService(AppDbContext db) => _db = db;

    public async Task<List<ProductCategoryResponse>> GetAllAsync()
    {
        var categories = await _db.ProductCategories
            .Include(c => c.Subcategories)
            .OrderBy(c => c.Order)
            .ToListAsync();

        return categories.Select(MapCategory).ToList();
    }

    public async Task<ProductCategoryResponse> CreateCategoryAsync(SaveProductCategoryRequest request)
    {
        var category = new ProductCategory
        {
            Name = request.Name,
            Order = request.Order,
        };
        _db.ProductCategories.Add(category);
        await _db.SaveChangesAsync();
        return MapCategory(category);
    }

    public async Task<ProductCategoryResponse> UpdateCategoryAsync(long id, SaveProductCategoryRequest request)
    {
        var category = await _db.ProductCategories
            .Include(c => c.Subcategories)
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new KeyNotFoundException($"Категорія {id} не знайдена");

        category.Name = request.Name;
        category.Order = request.Order;
        await _db.SaveChangesAsync();
        return MapCategory(category);
    }

    public async Task DeleteCategoryAsync(long id)
    {
        var category = await _db.ProductCategories.FindAsync(id)
            ?? throw new KeyNotFoundException($"Категорія {id} не знайдена");

        _db.ProductCategories.Remove(category);
        await _db.SaveChangesAsync();
    }

    public async Task<ProductSubcategoryResponse> CreateSubcategoryAsync(long categoryId, SaveProductSubcategoryRequest request)
    {
        var sub = new ProductSubcategory
        {
            CategoryId = categoryId,
            Name = request.Name,
            Order = request.Order,
        };
        _db.ProductSubcategories.Add(sub);
        await _db.SaveChangesAsync();
        return MapSubcategory(sub);
    }

    public async Task<ProductSubcategoryResponse> UpdateSubcategoryAsync(long id, SaveProductSubcategoryRequest request)
    {
        var sub = await _db.ProductSubcategories.FindAsync(id)
            ?? throw new KeyNotFoundException($"Підкатегорія {id} не знайдена");

        sub.Name = request.Name;
        sub.Order = request.Order;
        await _db.SaveChangesAsync();
        return MapSubcategory(sub);
    }

    public async Task DeleteSubcategoryAsync(long id)
    {
        var sub = await _db.ProductSubcategories.FindAsync(id)
            ?? throw new KeyNotFoundException($"Підкатегорія {id} не знайдена");

        _db.ProductSubcategories.Remove(sub);
        await _db.SaveChangesAsync();
    }

    private static ProductCategoryResponse MapCategory(ProductCategory c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        Order = c.Order,
        Subcategories = c.Subcategories.OrderBy(s => s.Order).Select(MapSubcategory).ToList(),
    };

    private static ProductSubcategoryResponse MapSubcategory(ProductSubcategory s) => new()
    {
        Id = s.Id,
        CategoryId = s.CategoryId,
        Name = s.Name,
        Order = s.Order,
    };
}
