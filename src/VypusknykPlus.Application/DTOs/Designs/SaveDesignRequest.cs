using VypusknykPlus.Application.ValueObjects;

namespace VypusknykPlus.Application.DTOs.Designs;

public class SaveDesignRequest
{
    public string DesignName { get; set; } = string.Empty;
    public RibbonState State { get; set; } = new();
}
