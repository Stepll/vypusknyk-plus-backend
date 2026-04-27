namespace VypusknykPlus.Application.DTOs.Admin;

public class PaymentMethodResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}

public class UpdatePaymentMethodRequest
{
    public bool IsEnabled { get; set; }
}
