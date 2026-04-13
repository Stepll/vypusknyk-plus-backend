namespace VypusknykPlus.Application.Services;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string fullName, string resetToken);
    Task SendOrderConfirmationEmailAsync(string toEmail, string fullName, string orderNumber, decimal total);
}
