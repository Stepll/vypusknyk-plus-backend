namespace VypusknykPlus.Application.DTOs.Admin;

public class SaveProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MinOrder { get; set; } = 1;
    public string Category { get; set; } = string.Empty;
    public string? Color { get; set; }
    public string[] Tags { get; set; } = [];
    public bool Popular { get; set; }
    public bool IsNew { get; set; }
    public bool IsDeleted { get; set; }
}
