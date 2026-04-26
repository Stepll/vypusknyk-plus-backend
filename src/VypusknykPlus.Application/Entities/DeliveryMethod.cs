namespace VypusknykPlus.Application.Entities;

public class DeliveryMethod : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public string Settings { get; set; } = "{}";
    public string CheckoutFields { get; set; } = "[]";

    public ICollection<Order> Orders { get; set; } = [];
}
