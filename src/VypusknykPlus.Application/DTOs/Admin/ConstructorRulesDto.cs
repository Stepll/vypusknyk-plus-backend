namespace VypusknykPlus.Application.DTOs.Admin;

public class ConstructorIncompatibilityResponse
{
    public long Id { get; set; }
    public string TypeA { get; set; } = string.Empty;
    public string SlugA { get; set; } = string.Empty;
    public string TypeB { get; set; } = string.Empty;
    public bool IsWarning { get; set; }
    public string? Message { get; set; }
    public List<string> SlugsB { get; set; } = [];
}

public class SaveConstructorIncompatibilityRequest
{
    public string TypeA { get; set; } = string.Empty;
    public string SlugA { get; set; } = string.Empty;
    public string TypeB { get; set; } = string.Empty;
    public bool IsWarning { get; set; }
    public string? Message { get; set; }
    public List<string> SlugsB { get; set; } = [];
}

public class ConstructorForcedTextResponse
{
    public long Id { get; set; }
    public string TriggerType { get; set; } = string.Empty;
    public string TriggerSlug { get; set; } = string.Empty;
    public string TargetField { get; set; } = string.Empty;
    public string? Message { get; set; }
    public List<string> Values { get; set; } = [];
}

public class SaveConstructorForcedTextRequest
{
    public string TriggerType { get; set; } = string.Empty;
    public string TriggerSlug { get; set; } = string.Empty;
    public string TargetField { get; set; } = string.Empty;
    public string? Message { get; set; }
    public List<string> Values { get; set; } = [];
}

public class ConstructorRulesResponse
{
    public List<ConstructorIncompatibilityResponse> Incompatibilities { get; set; } = [];
    public List<ConstructorForcedTextResponse> ForcedTexts { get; set; } = [];
}
