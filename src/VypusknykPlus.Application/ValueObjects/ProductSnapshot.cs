namespace VypusknykPlus.Application.ValueObjects;

public class ProductSnapshot
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Color { get; set; }
}
