using VypusknykPlus.Application.DTOs.Cart;

namespace VypusknykPlus.Application.Services;

public interface ICartService
{
    Task<CartItemResponse> AddAsync(long userId, AddCartItemRequest request);
    Task<List<CartItemResponse>> GetUserCartAsync(long userId);
    Task<CartItemResponse> UpdateQtyAsync(long userId, long itemId, UpdateCartItemRequest request);
    Task DeleteAsync(long userId, long itemId);
    Task ClearAsync(long userId);
}
