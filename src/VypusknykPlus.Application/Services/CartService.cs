using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Cart;
using VypusknykPlus.Application.Entities;
using VypusknykPlus.Application.ValueObjects;

namespace VypusknykPlus.Application.Services;

public class CartService : ICartService
{
    private readonly AppDbContext _db;
    private readonly ILogger<CartService> _logger;

    public CartService(AppDbContext db, ILogger<CartService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<CartItemResponse> AddAsync(long userId, AddCartItemRequest request)
    {
        ProductSnapshot snapshot;
        int? productId = null;

        if (request.ProductId.HasValue)
        {
            var product = await _db.Products.FindAsync(request.ProductId.Value)
                ?? throw new KeyNotFoundException("Продукт не знайдено");
            productId = product.Id;
            snapshot = new ProductSnapshot
            {
                Name = product.Name,
                Price = product.Price,
                Category = product.Category.ToString().ToLower(),
                Color = product.Color
            };
        }
        else
        {
            snapshot = new ProductSnapshot
            {
                Name = request.Name!,
                Price = request.Price!.Value,
                Category = "ribbon"
            };
        }

        var cartItem = new CartItem
        {
            Id = Guid.NewGuid(),
            Qty = request.Qty,
            ProductId = productId,
            ProductSnapshot = snapshot,
            NamesData = request.NamesData,
            RibbonCustomization = request.RibbonCustomization,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.CartItems.Add(cartItem);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Cart item {ItemId} added for user {UserId}, productId {ProductId}",
            cartItem.Id, userId, productId?.ToString() ?? "custom");

        return MapToResponse(cartItem);
    }

    public async Task<List<CartItemResponse>> GetUserCartAsync(long userId)
    {
        return await _db.CartItems
            .Where(ci => ci.UserId == userId)
            .OrderBy(ci => ci.CreatedAt)
            .Select(ci => MapToResponse(ci))
            .ToListAsync();
    }

    public async Task<CartItemResponse> UpdateQtyAsync(long userId, Guid itemId, UpdateCartItemRequest request)
    {
        var cartItem = await _db.CartItems
            .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.UserId == userId)
            ?? throw new KeyNotFoundException("Елемент кошика не знайдено");

        cartItem.Qty = request.Qty;
        cartItem.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        _logger.LogInformation("Cart item {ItemId} qty updated to {Qty} for user {UserId}",
            itemId, request.Qty, userId);

        return MapToResponse(cartItem);
    }

    public async Task DeleteAsync(long userId, Guid itemId)
    {
        var cartItem = await _db.CartItems
            .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.UserId == userId)
            ?? throw new KeyNotFoundException("Елемент кошика не знайдено");

        _db.CartItems.Remove(cartItem);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Cart item {ItemId} removed for user {UserId}", itemId, userId);
    }

    public async Task ClearAsync(long userId)
    {
        var items = await _db.CartItems
            .Where(ci => ci.UserId == userId)
            .ToListAsync();

        _db.CartItems.RemoveRange(items);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Cart cleared for user {UserId}, {Count} items removed", userId, items.Count);
    }

    private static CartItemResponse MapToResponse(CartItem ci) => new()
    {
        Id = ci.Id,
        ProductId = ci.ProductId,
        ProductName = ci.ProductSnapshot?.Name ?? string.Empty,
        ProductCategory = ci.ProductSnapshot?.Category ?? string.Empty,
        ProductColor = ci.ProductSnapshot?.Color,
        BasePrice = ci.ProductSnapshot?.Price ?? 0,
        Qty = ci.Qty,
        NamesData = ci.NamesData,
        RibbonCustomization = ci.RibbonCustomization
    };
}
