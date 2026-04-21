namespace VypusknykPlus.Application.Entities;

public class Supplier
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? TaxId { get; set; }
    public string? Address { get; set; }
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<Delivery> Deliveries { get; set; } = [];
}
