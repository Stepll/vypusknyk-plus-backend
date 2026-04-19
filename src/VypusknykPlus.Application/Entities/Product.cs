namespace VypusknykPlus.Application.Entities;

public enum ProductCategory
{
    Ribbon,
    Medal,
    Certificate,
    Accessory
}

public class Product
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ProductCategory Category { get; set; }
    public string? Color { get; set; }
    public decimal Price { get; set; }
    public int MinOrder { get; set; } = 1;
    public bool Popular { get; set; }
    public bool IsNew { get; set; }
    public string Description { get; set; } = string.Empty;
    public string[] Tags { get; set; } = [];
    public string? ImageKey { get; set; }

    public ICollection<ProductImage> Images { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
