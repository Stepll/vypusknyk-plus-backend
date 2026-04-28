namespace VypusknykPlus.Application.Entities;

public class ConstructorIncompatibility : BaseEntity
{
    public string TypeA { get; set; } = string.Empty;
    public string SlugA { get; set; } = string.Empty;
    public string TypeB { get; set; } = string.Empty;
    public bool IsWarning { get; set; } = false;
    public string? Message { get; set; }

    public List<ConstructorIncompatibilityTarget> Targets { get; set; } = [];
}
