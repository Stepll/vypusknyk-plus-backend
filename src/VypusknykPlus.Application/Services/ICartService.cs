using VypusknykPlus.Application.DTOs.Cart;

namespace VypusknykPlus.Application.Services;

public interface ICartService
{
    Task<CartItemResponse> AddAsync(Guid userId, AddCartItemRequest request);
    Task<List<CartItemResponse>> GetUserCartAsync(Guid userId);
    Task<CartItemResponse> UpdateQtyAsync(Guid userId, Guid itemId, UpdateCartItemRequest request);
    Task DeleteAsync(Guid userId, Guid itemId);
    Task ClearAsync(Guid userId);
}
