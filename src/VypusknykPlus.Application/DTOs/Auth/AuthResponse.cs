namespace VypusknykPlus.Application.DTOs.Auth;

public class AuthResponse
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool IsEmailVerified { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
