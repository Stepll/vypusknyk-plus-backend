namespace VypusknykPlus.Application.Entities;

public class CertificateTemplate : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ImageKey { get; set; }
    public decimal PriceModifier { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; } = 0;
    public string NativeOrientation { get; set; } = "portrait";
    public bool HasSecondSigner { get; set; } = false;
    public bool HasAdditionalText { get; set; } = false;
    public string? LayoutJson { get; set; }
}
