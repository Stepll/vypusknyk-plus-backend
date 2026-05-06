using System.Text.Json;

namespace VypusknykPlus.Application.DTOs.Designs;

public class BadgeDesignResponse
{
    public long Id { get; set; }
    public string DesignName { get; set; } = string.Empty;
    public DateTime SavedAt { get; set; }
    public JsonElement State { get; set; }
}
