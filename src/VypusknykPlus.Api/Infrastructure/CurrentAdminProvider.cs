using System.Security.Claims;
using VypusknykPlus.Application.Data;

namespace VypusknykPlus.Api.Infrastructure;

public class CurrentAdminProvider : ICurrentAdminProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentAdminProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public (long? AdminId, string AdminName) GetCurrent()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
            return (null, "Система");

        var idStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
        var name = user.FindFirstValue(ClaimTypes.Name) ?? "Admin";

        return long.TryParse(idStr, out var id) ? (id, name) : (null, "Система");
    }
}
