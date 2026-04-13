namespace VypusknykPlus.Application.DTOs.Products;

public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Color { get; set; }
    public decimal Price { get; set; }
    public int MinOrder { get; set; }
    public bool Popular { get; set; }
    public bool IsNew { get; set; }
    public string Description { get; set; } = string.Empty;
    public string[] Tags { get; set; } = [];
    public string? ImageUrl { get; set; }
}
