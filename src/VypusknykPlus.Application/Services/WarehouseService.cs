using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Services;

public class WarehouseService(AppDbContext db) : IWarehouseService
{
    public async Task<List<StockCategoryResponse>> GetCategoriesAsync()
    {
        return await db.StockCategories
            .OrderBy(c => c.Order)
            .Select(c => new StockCategoryResponse { Id = c.Id, Name = c.Name, Order = c.Order })
            .ToListAsync();
    }

    public async Task<List<StockSubcategoryResponse>> GetSubcategoriesAsync(long? categoryId)
    {
        var q = db.StockSubcategories.AsQueryable();
        if (categoryId.HasValue)
            q = q.Where(s => s.CategoryId == categoryId.Value);
        return await q
            .OrderBy(s => s.Order)
            .Select(s => new StockSubcategoryResponse
            {
                Id = s.Id, CategoryId = s.CategoryId, Name = s.Name, Order = s.Order,
            })
            .ToListAsync();
    }

    public async Task<WarehouseStatsResponse> GetStatsAsync()
    {
        var stockByVariant = await ComputeStockByVariantAsync();
        var productCount = await db.StockProducts.CountAsync();
        var categoryCount = await db.StockCategories.CountAsync();

        return new WarehouseStatsResponse
        {
            TotalStock = stockByVariant.Values.Sum(),
            OutOfStockCount = stockByVariant.Values.Count(s => s <= 0),
            LowStockCount = stockByVariant.Values.Count(s => s > 0 && s < 10),
            CategoryCount = categoryCount,
            ProductCount = productCount,
        };
    }

    public async Task<PagedResponse<StockProductSummary>> GetProductsAsync(WarehouseProductsQuery query)
    {
        var q = db.StockProducts
            .Include(p => p.Subcategory)
                .ThenInclude(s => s.Category)
            .Include(p => p.Variants)
            .AsQueryable();

        if (query.CategoryId.HasValue)
            q = q.Where(p => p.Subcategory.CategoryId == query.CategoryId.Value);

        if (query.SubcategoryId.HasValue)
            q = q.Where(p => p.SubcategoryId == query.SubcategoryId.Value);

        if (!string.IsNullOrWhiteSpace(query.Material))
            q = q.Where(p => p.Variants.Any(v => v.Material == query.Material));

        if (!string.IsNullOrWhiteSpace(query.Color))
            q = q.Where(p => p.Variants.Any(v => v.Color == query.Color));

        if (!string.IsNullOrWhiteSpace(query.Search))
            q = q.Where(p => p.Name.Contains(query.Search));

        var total = await q.CountAsync();

        var products = await q
            .OrderBy(p => p.Subcategory.Category.Order)
            .ThenBy(p => p.Subcategory.Order)
            .ThenBy(p => p.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var variantIds = products.SelectMany(p => p.Variants.Select(v => v.Id)).ToList();
        var stockDict = await ComputeStockForVariantsAsync(variantIds);

        var items = products.Select(p =>
        {
            var variants = string.IsNullOrWhiteSpace(query.Material)
                ? p.Variants.ToList()
                : p.Variants.Where(v => v.Material == query.Material).ToList();

            var totalStock = variants.Sum(v => stockDict.GetValueOrDefault(v.Id));
            var status = totalStock == 0 ? "out_of_stock"
                : totalStock < 10 ? "low_stock"
                : "in_stock";

            if (!string.IsNullOrWhiteSpace(query.Status) && status != query.Status)
                return null;

            return new StockProductSummary
            {
                Id = p.Id,
                SubcategoryId = p.SubcategoryId,
                SubcategoryName = p.Subcategory.Name,
                CategoryId = p.Subcategory.CategoryId,
                CategoryName = p.Subcategory.Category.Name,
                Name = p.Name,
                HasColor = p.HasColor,
                HasMaterial = p.HasMaterial,
                TotalStock = totalStock,
                VariantCount = variants.Count,
                Status = status,
            };
        })
        .Where(x => x != null)
        .Cast<StockProductSummary>()
        .ToList();

        return new PagedResponse<StockProductSummary>
        {
            Items = items,
            Total = total,
            Page = query.Page,
            PageSize = query.PageSize,
        };
    }

    public async Task<StockProductDetail?> GetProductDetailAsync(long id)
    {
        var product = await db.StockProducts
            .Include(p => p.Subcategory)
                .ThenInclude(s => s.Category)
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return null;

        var variantIds = product.Variants.Select(v => v.Id).ToList();
        var stockDict = await ComputeStockForVariantsAsync(variantIds);

        var transactions = await db.StockTransactions
            .Where(t => variantIds.Contains(t.VariantId))
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt)
            .ToListAsync();

        var variantMap = product.Variants.ToDictionary(v => v.Id);

        return new StockProductDetail
        {
            Id = product.Id,
            SubcategoryId = product.SubcategoryId,
            SubcategoryName = product.Subcategory.Name,
            CategoryId = product.Subcategory.CategoryId,
            CategoryName = product.Subcategory.Category.Name,
            Name = product.Name,
            HasColor = product.HasColor,
            HasMaterial = product.HasMaterial,
            Variants = product.Variants.Select(v => new StockVariantResponse
            {
                Id = v.Id,
                Material = v.Material,
                Color = v.Color,
                CurrentStock = stockDict.GetValueOrDefault(v.Id),
            }).ToList(),
            Transactions = transactions.Select(t =>
            {
                var v = variantMap[t.VariantId];
                return new StockTransactionResponse
                {
                    Id = t.Id,
                    VariantId = t.VariantId,
                    DeliveryItemId = t.DeliveryItemId,
                    Material = v.Material,
                    Color = v.Color,
                    Type = t.Type,
                    Quantity = t.Quantity,
                    Date = t.Date.ToString("yyyy-MM-dd"),
                    Note = t.Note,
                    CreatedAt = t.CreatedAt.ToString("o"),
                };
            }).ToList(),
        };
    }

    public async Task<StockTransactionResponse> AddTransactionAsync(CreateStockTransactionRequest request)
    {
        if (request.Quantity <= 0)
            throw new ArgumentException("Кількість має бути більше 0.");

        var variant = await db.StockVariants
            .FirstOrDefaultAsync(v =>
                v.ProductId == request.ProductId &&
                v.Material == request.Material &&
                v.Color == request.Color);

        if (variant == null)
        {
            if (request.Type == "outcome")
                throw new InvalidOperationException("Неможливо зареєструвати видачу: цей варіант відсутній на складі.");

            variant = new StockVariant
            {
                ProductId = request.ProductId,
                Material = request.Material,
                Color = request.Color,
            };
            db.StockVariants.Add(variant);
            await db.SaveChangesAsync();
        }

        if (request.Type == "outcome")
        {
            var currentStock = await GetVariantStockAsync(variant.Id);
            if (request.Quantity > currentStock)
                throw new InvalidOperationException(
                    $"Недостатньо товару. На складі: {currentStock}, запитано: {request.Quantity}.");
        }

        if (!DateTime.TryParse(request.Date, out var date))
            date = DateTime.UtcNow;

        var transaction = new StockTransaction
        {
            VariantId = variant.Id,
            Type = request.Type,
            Quantity = request.Quantity,
            Date = DateTime.SpecifyKind(date, DateTimeKind.Utc),
            Note = request.Note,
            CreatedAt = DateTime.UtcNow,
        };

        db.StockTransactions.Add(transaction);
        await db.SaveChangesAsync();

        return new StockTransactionResponse
        {
            Id = transaction.Id,
            VariantId = variant.Id,
            DeliveryItemId = null,
            Material = variant.Material,
            Color = variant.Color,
            Type = transaction.Type,
            Quantity = transaction.Quantity,
            Date = transaction.Date.ToString("yyyy-MM-dd"),
            Note = transaction.Note,
            CreatedAt = transaction.CreatedAt.ToString("o"),
        };
    }

    public async Task<StockProductSummary> CreateProductAsync(CreateStockProductRequest request)
    {
        var subcategory = await db.StockSubcategories
            .Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.Id == request.SubcategoryId)
            ?? throw new KeyNotFoundException($"Підкатегорію {request.SubcategoryId} не знайдено.");

        var product = new StockProduct
        {
            SubcategoryId = request.SubcategoryId,
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            HasColor = request.HasColor,
            HasMaterial = request.HasMaterial,
        };

        db.StockProducts.Add(product);
        await db.SaveChangesAsync();

        return new StockProductSummary
        {
            Id = product.Id,
            SubcategoryId = product.SubcategoryId,
            SubcategoryName = subcategory.Name,
            CategoryId = subcategory.CategoryId,
            CategoryName = subcategory.Category.Name,
            Name = product.Name,
            HasColor = product.HasColor,
            HasMaterial = product.HasMaterial,
            TotalStock = 0,
            VariantCount = 0,
            Status = "out_of_stock",
        };
    }

    private async Task<int> GetVariantStockAsync(long variantId)
    {
        var income = await db.StockTransactions
            .Where(t => t.VariantId == variantId && t.Type == "income")
            .SumAsync(t => (int?)t.Quantity) ?? 0;
        var outcome = await db.StockTransactions
            .Where(t => t.VariantId == variantId && t.Type == "outcome")
            .SumAsync(t => (int?)t.Quantity) ?? 0;
        return income - outcome;
    }

    private async Task<Dictionary<long, int>> ComputeStockForVariantsAsync(List<long> variantIds)
    {
        if (variantIds.Count == 0) return [];

        var rows = await db.StockTransactions
            .Where(t => variantIds.Contains(t.VariantId))
            .GroupBy(t => new { t.VariantId, t.Type })
            .Select(g => new { g.Key.VariantId, g.Key.Type, Total = g.Sum(t => t.Quantity) })
            .ToListAsync();

        return variantIds.ToDictionary(
            id => id,
            id =>
            {
                var inc = rows.FirstOrDefault(r => r.VariantId == id && r.Type == "income")?.Total ?? 0;
                var out_ = rows.FirstOrDefault(r => r.VariantId == id && r.Type == "outcome")?.Total ?? 0;
                return inc - out_;
            });
    }

    private async Task<Dictionary<long, int>> ComputeStockByVariantAsync()
    {
        var rows = await db.StockTransactions
            .GroupBy(t => new { t.VariantId, t.Type })
            .Select(g => new { g.Key.VariantId, g.Key.Type, Total = g.Sum(t => t.Quantity) })
            .ToListAsync();

        return rows
            .GroupBy(r => r.VariantId)
            .ToDictionary(
                g => g.Key,
                g =>
                {
                    var inc = g.FirstOrDefault(r => r.Type == "income")?.Total ?? 0;
                    var out_ = g.FirstOrDefault(r => r.Type == "outcome")?.Total ?? 0;
                    return inc - out_;
                });
    }
}
