using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Route("api/v1/order-statuses")]
public class OrderStatusesController : ControllerBase
{
    private readonly AppDbContext _db;

    public OrderStatusesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<OrderStatusResponse>>> GetActive()
    {
        var statuses = await _db.OrderStatuses
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.SortOrder)
            .ToListAsync();

        return Ok(statuses.Select(s => new OrderStatusResponse
        {
            Id = s.Id,
            Name = s.Name,
            Color = s.Color,
            SortOrder = s.SortOrder,
            IsFinal = s.IsFinal,
            IsActive = s.IsActive,
        }).ToList());
    }
}
