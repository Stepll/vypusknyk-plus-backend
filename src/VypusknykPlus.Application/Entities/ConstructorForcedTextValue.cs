namespace VypusknykPlus.Application.Entities;

public class ConstructorForcedTextValue
{
    public long Id { get; set; }
    public long RuleId { get; set; }
    public string Value { get; set; } = string.Empty;

    public ConstructorForcedText Rule { get; set; } = null!;
}
