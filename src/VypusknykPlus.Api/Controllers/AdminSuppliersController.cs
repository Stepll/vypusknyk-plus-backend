using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin/suppliers")]
public class AdminSuppliersController(IDeliveryService deliveries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SupplierResponse>>> GetSuppliers()
        => Ok(await deliveries.GetSuppliersAsync());

    [HttpPost]
    public async Task<ActionResult<SupplierResponse>> CreateSupplier([FromBody] SaveSupplierRequest request)
    {
        var result = await deliveries.CreateSupplierAsync(request);
        return Created(string.Empty, result);
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<SupplierResponse>> UpdateSupplier(long id, [FromBody] SaveSupplierRequest request)
        => Ok(await deliveries.UpdateSupplierAsync(id, request));

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteSupplier(long id)
    {
        await deliveries.DeleteSupplierAsync(id);
        return NoContent();
    }
}
