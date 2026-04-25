namespace VypusknykPlus.Application.DTOs.Products;

public class ProductResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public long? SubcategoryId { get; set; }
    public string? SubcategoryName { get; set; }
    public string? Color { get; set; }
    public decimal Price { get; set; }
    public int MinOrder { get; set; }
    public bool Popular { get; set; }
    public bool IsNew { get; set; }
    public string Description { get; set; } = string.Empty;
    public string[] Tags { get; set; } = [];
    public string? ImageUrl { get; set; }
}
