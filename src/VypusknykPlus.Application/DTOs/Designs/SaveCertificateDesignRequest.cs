using System.Text.Json;

namespace VypusknykPlus.Application.DTOs.Designs;

public class SaveCertificateDesignRequest
{
    public string DesignName { get; set; } = string.Empty;
    public JsonElement State { get; set; }
}
