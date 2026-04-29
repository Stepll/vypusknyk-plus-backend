using VypusknykPlus.Application.DTOs.Auth;

namespace VypusknykPlus.Application.Services;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task<AuthResponse> UpdateProfileAsync(long userId, UpdateProfileRequest request);
    Task ChangePasswordAsync(long userId, ChangePasswordRequest request);
    Task ForgotPasswordAsync(ForgotPasswordRequest request);
    Task ResetPasswordAsync(ResetPasswordRequest request);
    Task VerifyEmailAsync(string token);
    Task ResendActivationEmailAsync(long userId);
}
