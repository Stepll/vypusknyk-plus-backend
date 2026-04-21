using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/deliveries")]
public class AdminDeliveriesController(IDeliveryService deliveries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<DeliverySummary>>> GetDeliveries([FromQuery] DeliveryQuery query)
        => Ok(await deliveries.GetDeliveriesAsync(query));

    [HttpGet("{id:long}")]
    public async Task<ActionResult<DeliveryDetail>> GetDelivery(long id)
    {
        var result = await deliveries.GetDeliveryDetailAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<DeliveryDetail>> CreateDelivery([FromBody] CreateDeliveryRequest request)
    {
        var result = await deliveries.CreateDeliveryAsync(request);
        return Created(string.Empty, result);
    }

    [HttpPost("{deliveryId:long}/items/{itemId:long}/receive")]
    public async Task<ActionResult<DeliveryItemResponse>> ReceiveItem(
        long deliveryId, long itemId, [FromBody] ReceiveDeliveryItemRequest request)
    {
        var result = await deliveries.ReceiveItemAsync(deliveryId, itemId, request);
        return Ok(result);
    }

    [HttpPost("{deliveryId:long}/receive-all")]
    public async Task<IActionResult> ReceiveAll(long deliveryId, [FromBody] ReceiveAllRequest request)
    {
        await deliveries.ReceiveAllAsync(deliveryId, request);
        return NoContent();
    }
}
