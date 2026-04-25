namespace VypusknykPlus.Application.Entities;

public class ProductCategory
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public ICollection<ProductSubcategory> Subcategories { get; set; } = [];
    public ICollection<Product> Products { get; set; } = [];
}
