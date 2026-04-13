using VypusknykPlus.Application.DTOs.Orders;

namespace VypusknykPlus.Application.Services;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(Guid userId, CreateOrderRequest request);
    Task<OrderListResponse> GetUserOrdersAsync(Guid userId);
    Task<OrderResponse?> GetByIdAsync(Guid userId, Guid orderId);
}
