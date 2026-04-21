using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Services;

public class DeliveryService(AppDbContext db) : IDeliveryService
{
    public async Task<List<SupplierResponse>> GetSuppliersAsync() =>
        await db.Suppliers
            .OrderBy(s => s.Name)
            .Select(s => MapSupplier(s))
            .ToListAsync();

    public async Task<SupplierResponse> CreateSupplierAsync(SaveSupplierRequest request)
    {
        var supplier = new Supplier
        {
            Name = request.Name.Trim(),
            ContactPerson = request.ContactPerson?.Trim(),
            Phone = request.Phone?.Trim(),
            Email = request.Email?.Trim(),
            TaxId = request.TaxId?.Trim(),
            Address = request.Address?.Trim(),
            Notes = request.Notes?.Trim(),
        };
        db.Suppliers.Add(supplier);
        await db.SaveChangesAsync();
        return MapSupplier(supplier);
    }

    public async Task<SupplierResponse> UpdateSupplierAsync(long id, SaveSupplierRequest request)
    {
        var supplier = await db.Suppliers.FindAsync(id)
            ?? throw new KeyNotFoundException("Постачальника не знайдено.");
        supplier.Name = request.Name.Trim();
        supplier.ContactPerson = request.ContactPerson?.Trim();
        supplier.Phone = request.Phone?.Trim();
        supplier.Email = request.Email?.Trim();
        supplier.TaxId = request.TaxId?.Trim();
        supplier.Address = request.Address?.Trim();
        supplier.Notes = request.Notes?.Trim();
        await db.SaveChangesAsync();
        return MapSupplier(supplier);
    }

    public async Task DeleteSupplierAsync(long id)
    {
        var supplier = await db.Suppliers.FindAsync(id)
            ?? throw new KeyNotFoundException("Постачальника не знайдено.");
        supplier.IsDeleted = true;
        await db.SaveChangesAsync();
    }

    private static SupplierResponse MapSupplier(Supplier s) => new()
    {
        Id = s.Id, Name = s.Name, ContactPerson = s.ContactPerson,
        Phone = s.Phone, Email = s.Email, TaxId = s.TaxId,
        Address = s.Address, Notes = s.Notes,
    };

    public async Task<PagedResponse<DeliverySummary>> GetDeliveriesAsync(DeliveryQuery query)
    {
        var q = db.Deliveries
            .Include(d => d.Supplier)
            .Include(d => d.Items)
            .AsQueryable();

        if (query.SupplierId.HasValue)
            q = q.Where(d => d.SupplierId == query.SupplierId.Value);

        if (!string.IsNullOrWhiteSpace(query.Status))
            q = q.Where(d => d.Status == query.Status);

        if (!string.IsNullOrWhiteSpace(query.Search))
            q = q.Where(d => d.Number.Contains(query.Search) ||
                (d.Supplier != null && d.Supplier.Name.Contains(query.Search)));

        var total = await q.CountAsync();

        var deliveries = await q
            .OrderByDescending(d => d.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PagedResponse<DeliverySummary>
        {
            Items = deliveries.Select(MapToSummary).ToList(),
            Total = total,
            Page = query.Page,
            PageSize = query.PageSize,
        };
    }

    public async Task<DeliveryDetail?> GetDeliveryDetailAsync(long id)
    {
        var delivery = await db.Deliveries
            .Include(d => d.Supplier)
            .Include(d => d.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Subcategory)
                        .ThenInclude(s => s.Category)
            .FirstOrDefaultAsync(d => d.Id == id);

        return delivery is null ? null : MapToDetail(delivery);
    }

    public async Task<DeliveryDetail> CreateDeliveryAsync(CreateDeliveryRequest request)
    {
        if (request.Items == null || request.Items.Count == 0)
            throw new ArgumentException("Поставка має містити принаймні одну позицію.");

        var number = await GenerateNumberAsync();

        if (!DateTime.TryParse(request.ExpectedDate, out var expectedDate))
            expectedDate = DateTime.UtcNow;

        var delivery = new Delivery
        {
            Number = number,
            SupplierId = request.SupplierId,
            ExpectedDate = DateTime.SpecifyKind(expectedDate, DateTimeKind.Utc),
            Status = "pending",
            Note = request.Note?.Trim(),
            CreatedAt = DateTime.UtcNow,
        };

        db.Deliveries.Add(delivery);
        await db.SaveChangesAsync();

        foreach (var item in request.Items)
        {
            db.DeliveryItems.Add(new DeliveryItem
            {
                DeliveryId = delivery.Id,
                ProductId = item.ProductId,
                Material = item.Material ?? string.Empty,
                Color = item.Color ?? string.Empty,
                ExpectedQty = item.ExpectedQty,
            });
        }

        await db.SaveChangesAsync();
        return (await GetDeliveryDetailAsync(delivery.Id))!;
    }

    public async Task<DeliveryItemResponse> ReceiveItemAsync(
        long deliveryId, long itemId, ReceiveDeliveryItemRequest request)
    {
        var item = await db.DeliveryItems
            .Include(i => i.Delivery)
            .Include(i => i.Product).ThenInclude(p => p.Subcategory).ThenInclude(s => s.Category)
            .FirstOrDefaultAsync(i => i.Id == itemId && i.DeliveryId == deliveryId)
            ?? throw new KeyNotFoundException("Позицію не знайдено.");

        if (item.Delivery.IsDeleted)
            throw new InvalidOperationException("Поставку видалено.");

        var remaining = item.ExpectedQty - item.ReceivedQty;
        if (remaining <= 0)
            throw new InvalidOperationException("Позицію вже повністю прийнято.");

        if (request.Quantity <= 0 || request.Quantity > remaining)
            throw new InvalidOperationException($"Кількість має бути від 1 до {remaining}.");

        var variant = await FindOrCreateVariantAsync(item.ProductId, item.Material, item.Color);

        if (!DateTime.TryParse(request.Date, out var date))
            date = DateTime.UtcNow;

        db.StockTransactions.Add(new StockTransaction
        {
            VariantId = variant.Id,
            DeliveryItemId = item.Id,
            Type = "income",
            Quantity = request.Quantity,
            Date = DateTime.SpecifyKind(date, DateTimeKind.Utc),
            Note = request.Note ?? $"Поставка {item.Delivery.Number}",
            CreatedAt = DateTime.UtcNow,
        });

        item.ReceivedQty += request.Quantity;
        item.ReceivedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        await UpdateDeliveryStatusAsync(deliveryId);

        return MapItemToResponse(item);
    }

    public async Task ReceiveAllAsync(long deliveryId, ReceiveAllRequest request)
    {
        var delivery = await db.Deliveries
            .IgnoreQueryFilters()
            .Include(d => d.Items)
            .FirstOrDefaultAsync(d => d.Id == deliveryId && !d.IsDeleted)
            ?? throw new KeyNotFoundException("Поставку не знайдено.");

        if (!DateTime.TryParse(request.Date, out var date))
            date = DateTime.UtcNow;
        var utcDate = DateTime.SpecifyKind(date, DateTimeKind.Utc);

        foreach (var item in delivery.Items.Where(i => i.ReceivedQty < i.ExpectedQty))
        {
            var remaining = item.ExpectedQty - item.ReceivedQty;
            var variant = await FindOrCreateVariantAsync(item.ProductId, item.Material, item.Color);

            db.StockTransactions.Add(new StockTransaction
            {
                VariantId = variant.Id,
                DeliveryItemId = item.Id,
                Type = "income",
                Quantity = remaining,
                Date = utcDate,
                Note = $"Поставка {delivery.Number}",
                CreatedAt = DateTime.UtcNow,
            });

            item.ReceivedQty = item.ExpectedQty;
            item.ReceivedAt = DateTime.UtcNow;
        }

        delivery.Status = "received";
        await db.SaveChangesAsync();
    }

    private async Task<StockVariant> FindOrCreateVariantAsync(long productId, string material, string color)
    {
        var variant = await db.StockVariants
            .FirstOrDefaultAsync(v => v.ProductId == productId && v.Material == material && v.Color == color);

        if (variant is null)
        {
            variant = new StockVariant { ProductId = productId, Material = material, Color = color };
            db.StockVariants.Add(variant);
            await db.SaveChangesAsync();
        }

        return variant;
    }

    private async Task UpdateDeliveryStatusAsync(long deliveryId)
    {
        var items = await db.DeliveryItems.Where(i => i.DeliveryId == deliveryId).ToListAsync();
        var delivery = await db.Deliveries.FindAsync(deliveryId);
        if (delivery is null) return;

        delivery.Status = items.All(i => i.ReceivedQty >= i.ExpectedQty) ? "received"
            : items.Any(i => i.ReceivedQty > 0) ? "partial"
            : "pending";

        await db.SaveChangesAsync();
    }

    private async Task<string> GenerateNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var count = await db.Deliveries.IgnoreQueryFilters().CountAsync(d => d.CreatedAt.Year == year) + 1;
        return $"DEL-{year}-{count:D4}";
    }

    private static DeliverySummary MapToSummary(Delivery d) => new()
    {
        Id = d.Id,
        Number = d.Number,
        SupplierId = d.SupplierId,
        SupplierName = d.Supplier?.Name,
        ExpectedDate = d.ExpectedDate.ToString("yyyy-MM-dd"),
        Status = d.Status,
        Note = d.Note,
        ItemCount = d.Items.Count,
        TotalExpectedQty = d.Items.Sum(i => i.ExpectedQty),
        TotalReceivedQty = d.Items.Sum(i => i.ReceivedQty),
        CreatedAt = d.CreatedAt.ToString("o"),
    };

    private static DeliveryDetail MapToDetail(Delivery d) => new()
    {
        Id = d.Id,
        Number = d.Number,
        SupplierId = d.SupplierId,
        SupplierName = d.Supplier?.Name,
        ExpectedDate = d.ExpectedDate.ToString("yyyy-MM-dd"),
        Status = d.Status,
        Note = d.Note,
        CreatedAt = d.CreatedAt.ToString("o"),
        Items = d.Items.Select(MapItemToResponse).ToList(),
    };

    private static DeliveryItemResponse MapItemToResponse(DeliveryItem i) => new()
    {
        Id = i.Id,
        DeliveryId = i.DeliveryId,
        ProductId = i.ProductId,
        ProductName = i.Product?.Name ?? string.Empty,
        SubcategoryName = i.Product?.Subcategory?.Name ?? string.Empty,
        CategoryName = i.Product?.Subcategory?.Category?.Name ?? string.Empty,
        HasColor = i.Product?.HasColor ?? false,
        HasMaterial = i.Product?.HasMaterial ?? false,
        Material = i.Material,
        Color = i.Color,
        ExpectedQty = i.ExpectedQty,
        ReceivedQty = i.ReceivedQty,
        ReceivedAt = i.ReceivedAt?.ToString("yyyy-MM-dd"),
    };
}
