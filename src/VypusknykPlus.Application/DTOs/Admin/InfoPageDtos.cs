namespace VypusknykPlus.Application.DTOs.Admin;

public class InfoPageResponse
{
    public long Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int Order { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UpdateInfoPageRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
