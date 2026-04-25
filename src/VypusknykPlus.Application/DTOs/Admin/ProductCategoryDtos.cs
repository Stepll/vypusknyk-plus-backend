namespace VypusknykPlus.Application.DTOs.Admin;

public class ProductCategoryResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public List<ProductSubcategoryResponse> Subcategories { get; set; } = [];
}

public class ProductSubcategoryResponse
{
    public long Id { get; set; }
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class SaveProductCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class SaveProductSubcategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
}
