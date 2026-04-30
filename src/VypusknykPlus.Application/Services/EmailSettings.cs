namespace VypusknykPlus.Application.Services;

public class EmailSettings
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SmtpUser { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = "Випускник+";
    public string FrontendUrl { get; set; } = "http://localhost:5173";
    public string AdminPanelUrl { get; set; } = "http://localhost:5174";
}
