namespace VypusknykPlus.Application.DTOs.Admin;

public class AdminAuthResponse
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsSuperAdmin { get; set; }
    public string Token { get; set; } = string.Empty;
}
