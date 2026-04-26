namespace VypusknykPlus.Application.DTOs.Admin;

public class DeliveryCheckoutFieldDto
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = "input";
    public bool Required { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string OptionsJson { get; set; } = string.Empty;
}

public class DeliveryMethodResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string Settings { get; set; } = "{}";
    public List<DeliveryCheckoutFieldDto> CheckoutFields { get; set; } = [];
}

public class UpdateDeliveryMethodRequest
{
    public bool IsEnabled { get; set; }
    public string Settings { get; set; } = "{}";
    public List<DeliveryCheckoutFieldDto> CheckoutFields { get; set; } = [];
}
