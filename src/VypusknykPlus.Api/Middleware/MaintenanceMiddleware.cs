using System.Text.Json;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Api.Middleware;

public class MaintenanceMiddleware(RequestDelegate next)
{
    private static readonly JsonSerializerOptions _json = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task InvokeAsync(HttpContext context, IAppSettingsService appSettings)
    {
        var settings = await appSettings.GetPublicAsync();

        if (settings.TryGetValue("maintenance_mode", out var mode) && mode == "true")
        {
            // allow admin API and public settings through so frontend can still get the maintenance text
            var path = context.Request.Path.Value ?? "";
            var isExempt = path.StartsWith("/api/v1/admin") ||
                           path.StartsWith("/api/v1/settings") ||
                           path.StartsWith("/hubs/") ||
                           path == "/healthz";

            if (!isExempt)
            {
                var text = settings.TryGetValue("maintenance_text", out var t) ? t : "Сайт тимчасово недоступний. Спробуйте пізніше.";
                context.Response.StatusCode = 503;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { message = text }, _json));
                return;
            }
        }

        await next(context);
    }
}
