using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Application.Services;

public interface IDeliveryService
{
    Task<List<SupplierResponse>> GetSuppliersAsync();
    Task<PagedResponse<DeliverySummary>> GetDeliveriesAsync(DeliveryQuery query);
    Task<DeliveryDetail?> GetDeliveryDetailAsync(long id);
    Task<DeliveryDetail> CreateDeliveryAsync(CreateDeliveryRequest request);
    Task<DeliveryItemResponse> ReceiveItemAsync(long deliveryId, long itemId, ReceiveDeliveryItemRequest request);
    Task ReceiveAllAsync(long deliveryId, ReceiveAllRequest request);
}
