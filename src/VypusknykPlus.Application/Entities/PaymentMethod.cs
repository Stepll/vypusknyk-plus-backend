namespace VypusknykPlus.Application.Entities;

public class PaymentMethod : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;

    public ICollection<Order> Orders { get; set; } = [];
}
