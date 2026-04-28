namespace VypusknykPlus.Application.Entities;

public class ConstructorForcedText : BaseEntity
{
    public string TriggerType { get; set; } = string.Empty;
    public string TriggerSlug { get; set; } = string.Empty;
    public string TargetField { get; set; } = string.Empty;
    public string? Message { get; set; }

    public List<ConstructorForcedTextValue> Values { get; set; } = [];
}
