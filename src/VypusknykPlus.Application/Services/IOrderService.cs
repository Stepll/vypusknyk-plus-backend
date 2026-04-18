using VypusknykPlus.Application.DTOs.Orders;

namespace VypusknykPlus.Application.Services;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(long? userId, CreateOrderRequest request);
    Task<OrderListResponse> GetUserOrdersAsync(long userId);
    Task<OrderResponse?> GetByIdAsync(long userId, Guid orderId);
    Task<OrderListResponse> GetGuestOrdersAsync(string guestToken);
    Task ClaimGuestOrdersAsync(long userId, string userEmail, string? guestToken);
}
