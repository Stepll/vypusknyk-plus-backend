namespace VypusknykPlus.Application.Entities;

public class ConstructorIncompatibilityTarget
{
    public long Id { get; set; }
    public long RuleId { get; set; }
    public string SlugB { get; set; } = string.Empty;

    public ConstructorIncompatibility Rule { get; set; } = null!;
}
