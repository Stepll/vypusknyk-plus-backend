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
}
