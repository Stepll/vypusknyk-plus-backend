namespace VypusknykPlus.Application.DTOs.Admin;

public class BadgeTextSizeResponse
{
    public long Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public int Value { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class SaveBadgeTextSizeRequest
{
    public string Label { get; set; } = string.Empty;
    public int Value { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}
