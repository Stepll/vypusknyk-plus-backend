namespace VypusknykPlus.Application.Entities;

public class PageContent
{
    public string Slug { get; set; } = string.Empty;
    public string Data { get; set; } = "{}"; // JSON
    public DateTime UpdatedAt { get; set; }
}
