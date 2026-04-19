namespace VypusknykPlus.Application.DTOs.Admin;

public class AdminProductResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsDeleted { get; set; }
}
