namespace VypusknykPlus.Application.DTOs.Admin;

public class AdminProductDetailResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MinOrder { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string[] Tags { get; set; } = [];
    public bool Popular { get; set; }
    public bool IsNew { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ProductImageResponse> Images { get; set; } = [];
}
