using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VypusknykPlus.Application.DTOs.Designs;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class DesignsController : ControllerBase
{
    private readonly IDesignService _designService;

    public DesignsController(IDesignService designService)
    {
        _designService = designService;
    }

    private long GetUserId() => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<ActionResult<DesignResponse>> Save([FromBody] SaveDesignRequest request)
    {
        var response = await _designService.SaveAsync(GetUserId(), request);
        return Created(string.Empty, response);
    }

    [HttpGet]
    public async Task<ActionResult<List<DesignResponse>>> GetUserDesigns()
    {
        var response = await _designService.GetUserDesignsAsync(GetUserId());
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<DesignResponse>> Update(Guid id, [FromBody] SaveDesignRequest request)
    {
        var response = await _designService.UpdateAsync(GetUserId(), id, request);
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        await _designService.DeleteAsync(GetUserId(), id);
        return NoContent();
    }
}
