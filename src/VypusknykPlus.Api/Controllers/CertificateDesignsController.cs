using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VypusknykPlus.Application.DTOs.Designs;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/certificate-designs")]
public class CertificateDesignsController : ControllerBase
{
    private readonly ICertificateDesignService _service;

    public CertificateDesignsController(ICertificateDesignService service) => _service = service;

    private long GetUserId() => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<ActionResult<CertificateDesignResponse>> Save([FromBody] SaveCertificateDesignRequest request)
    {
        var response = await _service.SaveAsync(GetUserId(), request);
        return Created(string.Empty, response);
    }

    [HttpGet]
    public async Task<ActionResult<List<CertificateDesignResponse>>> GetUserDesigns()
    {
        return Ok(await _service.GetUserDesignsAsync(GetUserId()));
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<CertificateDesignResponse>> Update(long id, [FromBody] SaveCertificateDesignRequest request)
    {
        var response = await _service.UpdateAsync(GetUserId(), id, request);
        return Ok(response);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _service.DeleteAsync(GetUserId(), id);
        return NoContent();
    }
}
