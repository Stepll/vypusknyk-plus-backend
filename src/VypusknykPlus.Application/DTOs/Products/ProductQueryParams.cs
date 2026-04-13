namespace VypusknykPlus.Application.DTOs.Products;

public class ProductQueryParams
{
    public string? Category { get; set; }
    public string? Sort { get; set; }
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}
