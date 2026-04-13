namespace VypusknykPlus.Application.ValueObjects;

public enum DeliveryMethod
{
    NovaPoshta,
    Ukrposhta
}

public class DeliveryInfo
{
    public DeliveryMethod Method { get; set; }
    public string? City { get; set; }
    public string? Warehouse { get; set; }
    public string? PostalCode { get; set; }
}
