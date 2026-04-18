using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _db;
    private readonly IImageService _imageService;

    public AdminService(AppDbContext db, IImageService imageService)
    {
        _db = db;
        _imageService = imageService;
    }

    public async Task<PagedResponse<AdminOrderResponse>> GetOrdersAsync(int page, int pageSize, string? status)
    {
        var query = _db.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<OrderStatus>(status, true, out var parsed))
            query = query.Where(o => o.Status == parsed);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<AdminOrderResponse>
        {
            Items = items.Select(MapOrder).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task<AdminOrderResponse?> GetOrderAsync(Guid id)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);

        return order is null ? null : MapOrder(order);
    }

    public async Task UpdateOrderStatusAsync(Guid id, string status)
    {
        if (!Enum.TryParse<OrderStatus>(status, true, out var parsed))
            throw new ArgumentException($"Невідомий статус: {status}");

        var order = await _db.Orders.FindAsync(id)
            ?? throw new KeyNotFoundException($"Замовлення {id} не знайдено");

        order.Status = parsed;
        order.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<PagedResponse<AdminProductResponse>> GetProductsAsync(int page, int pageSize)
    {
        var query = _db.Products.IgnoreQueryFilters().AsNoTracking();

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<AdminProductResponse>
        {
            Items = items.Select(p => new AdminProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Category = p.Category.ToString(),
                ImageUrl = _imageService.GetPublicUrl(p.ImageKey),
                IsDeleted = p.IsDeleted,
            }).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _db.Products.FindAsync(id)
            ?? throw new KeyNotFoundException($"Продукт {id} не знайдено");

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<PagedResponse<AdminUserResponse>> GetUsersAsync(int page, int pageSize)
    {
        var total = await _db.Users.CountAsync();
        var items = await _db.Users
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new AdminUserResponse
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                CreatedAt = u.CreatedAt,
                OrdersCount = u.Orders.Count,
            })
            .ToListAsync();

        return new PagedResponse<AdminUserResponse>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task<AdminUserResponse?> GetUserAsync(long id)
    {
        return await _db.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new AdminUserResponse
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                CreatedAt = u.CreatedAt,
                OrdersCount = u.Orders.Count,
            })
            .FirstOrDefaultAsync();
    }

    private static AdminOrderResponse MapOrder(Order o) => new()
    {
        Id = o.Id,
        OrderNumber = o.OrderNumber,
        CreatedAt = o.CreatedAt,
        Status = o.Status.ToString(),
        Total = o.Total,
        IsAnonymous = o.IsAnonymous,
        UserId = o.UserId,
        Email = o.Email,
        Comment = o.Comment,
        Payment = o.Payment.ToString(),
        Recipient = new AdminRecipientResponse
        {
            FullName = o.Recipient.FullName,
            Phone = o.Recipient.Phone,
        },
        Delivery = new AdminDeliveryResponse
        {
            Method = o.Delivery.Method.ToString(),
            City = o.Delivery.City,
            Warehouse = o.Delivery.Warehouse,
        },
        Items = o.Items.Select(i => new AdminOrderItemResponse
        {
            Id = i.Id,
            Name = i.Name,
            Quantity = i.Quantity,
            Price = i.Price,
        }).ToList(),
    };
}
