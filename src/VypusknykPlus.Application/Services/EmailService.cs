using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace VypusknykPlus.Application.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string fullName, string resetToken)
    {
        var resetUrl = $"{_settings.FrontendUrl}/reset-password?token={Uri.EscapeDataString(resetToken)}";

        var body = $"""
            <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
                <h2 style="color: #e91e8c;">Випускник+</h2>
                <p>Вітаємо, {fullName}!</p>
                <p>Ви отримали цей лист, тому що запросили відновлення пароля.</p>
                <p>Натисніть кнопку нижче, щоб встановити новий пароль:</p>
                <p style="text-align: center; margin: 30px 0;">
                    <a href="{resetUrl}"
                       style="background-color: #e91e8c; color: white; padding: 12px 30px;
                              text-decoration: none; border-radius: 6px; font-size: 16px;">
                        Відновити пароль
                    </a>
                </p>
                <p style="color: #666; font-size: 14px;">
                    Посилання дійсне протягом 1 години. Якщо ви не запитували відновлення пароля, проігноруйте цей лист.
                </p>
            </div>
            """;

        await SendEmailAsync(toEmail, "Відновлення пароля — Випускник+", body);

        _logger.LogInformation("Password reset email sent to {Email}", toEmail);
    }

    public async Task SendOrderConfirmationEmailAsync(string toEmail, string fullName, string orderNumber, decimal total)
    {
        var body = $"""
            <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
                <h2 style="color: #e91e8c;">Випускник+</h2>
                <p>Вітаємо, {fullName}!</p>
                <p>Ваше замовлення <strong>{orderNumber}</strong> прийнято.</p>
                <p>Сума: <strong>{total} грн</strong></p>
                <p>Ми повідомимо вас, коли замовлення буде готове до відправки.</p>
                <p style="color: #666; font-size: 14px; margin-top: 30px;">
                    Дякуємо за замовлення!<br/>Команда Випускник+
                </p>
            </div>
            """;

        await SendEmailAsync(toEmail, $"Замовлення {orderNumber} прийнято — Випускник+", body);

        _logger.LogInformation("Order confirmation email sent to {Email}, order {OrderNumber}", toEmail, orderNumber);
    }

    public async Task SendActivationEmailAsync(string toEmail, string fullName, string verificationToken)
    {
        var verifyUrl = $"{_settings.FrontendUrl}/verify-email?token={Uri.EscapeDataString(verificationToken)}";

        var body = $"""
            <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
                <h2 style="color: #e91e8c;">Випускник+</h2>
                <p>Вітаємо, {fullName}!</p>
                <p>Дякуємо за реєстрацію. Для активації акаунту підтвердіть свою електронну адресу:</p>
                <p style="text-align: center; margin: 30px 0;">
                    <a href="{verifyUrl}"
                       style="background-color: #e91e8c; color: white; padding: 12px 30px;
                              text-decoration: none; border-radius: 6px; font-size: 16px;">
                        Підтвердити email
                    </a>
                </p>
                <p style="color: #666; font-size: 14px;">
                    Посилання дійсне 24 години. Якщо ви не реєструвалися — проігноруйте цей лист.
                </p>
                <p style="color: #666; font-size: 14px; margin-top: 30px;">
                    З повагою,<br/>Команда Випускник+
                </p>
            </div>
            """;

        await SendEmailAsync(toEmail, "Підтвердження email — Випускник+", body);

        _logger.LogInformation("Activation email sent to {Email}", toEmail);
    }

    public Task SendRawEmailAsync(string toEmail, string subject, string htmlBody)
        => SendEmailAsync(toEmail, subject, htmlBody);

    private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        message.Body = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
