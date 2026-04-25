namespace VypusknykPlus.Application.Entities;

public class ProductSubcategory
{
    public long Id { get; set; }
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public ProductCategory Category { get; set; } = null!;
    public ICollection<Product> Products { get; set; } = [];
}
