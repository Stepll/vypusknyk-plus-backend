namespace VypusknykPlus.Application.Entities;

public class ProductImage
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string ImageKey { get; set; } = string.Empty;
    public bool IsPreview { get; set; }
    public DateTime CreatedAt { get; set; }
}
