namespace VypusknykPlus.Application.DTOs.Admin;

public class ProductImageResponse
{
    public long Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPreview { get; set; }
}
