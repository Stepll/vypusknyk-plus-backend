using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Auth;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly JwtSettings _jwt;
    private readonly IEmailService _emailService;
    private readonly IOrderService _orderService;
    private readonly INotificationService _notifications;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext db, IOptions<JwtSettings> jwt, IEmailService emailService, IOrderService orderService, INotificationService notifications, IServiceScopeFactory scopeFactory, ILogger<AuthService> logger)
    {
        _db = db;
        _jwt = jwt.Value;
        _emailService = emailService;
        _orderService = orderService;
        _notifications = notifications;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsGuest);

        if (user is null || user.PasswordHash is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Невірний email або пароль");

        _logger.LogInformation("User {Email} logged in", user.Email);

        var loginEmail = user.Email!;
        var loginId = user.Id;
        _ = ClaimGuestOrdersInBackgroundAsync(loginId, loginEmail)
            .ContinueWith(t => _logger.LogError(t.Exception, "Failed to claim guest orders for {Email}", loginEmail),
                TaskContinuationOptions.OnlyOnFaulted);

        return await ToAuthResponse(user);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _db.Users.IgnoreQueryFilters().AnyAsync(u => u.Email == request.Email && !u.IsGuest))
            throw new ArgumentException("Користувач з таким email вже існує");

        var guestUser = request.Phone is not null
            ? await _db.Users.IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Phone == request.Phone && u.IsGuest && !u.IsDeleted)
            : null;

        User user;
        if (guestUser is not null)
        {
            guestUser.IsGuest = false;
            guestUser.Email = request.Email;
            guestUser.FullName = request.FullName;
            guestUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            guestUser.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            user = guestUser;
            _logger.LogInformation("Guest user {Phone} converted to registered user {Email}", request.Phone, user.Email);
        }
        else
        {
            user = new User
            {
                Email = request.Email,
                FullName = request.FullName,
                Phone = request.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            _logger.LogInformation("New user registered: {Email}", user.Email);
        }

        var capturedId = user.Id;
        var capturedEmail = user.Email!;
        var capturedFullName = user.FullName;

        _ = ClaimGuestOrdersInBackgroundAsync(capturedId, capturedEmail)
            .ContinueWith(t => _logger.LogError(t.Exception, "Failed to claim guest orders for {Email}", capturedEmail),
                TaskContinuationOptions.OnlyOnFaulted);

        _ = SendActivationEmailInBackgroundAsync(capturedId, capturedEmail, capturedFullName)
            .ContinueWith(t => _logger.LogError(t.Exception, "Failed to send activation email to {Email}", capturedEmail),
                TaskContinuationOptions.OnlyOnFaulted);

        var notifContext = new Dictionary<string, string>
        {
            ["fullName"] = capturedFullName,
            ["email"] = capturedEmail,
            ["phone"] = request.Phone ?? "",
            ["registrationDate"] = DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm"),
        };
        _ = _notifications.OnNewUserAsync(capturedId, notifContext)
            .ContinueWith(t => { }, TaskContinuationOptions.OnlyOnFaulted);

        return await ToAuthResponse(user);
    }

    private async Task ClaimGuestOrdersInBackgroundAsync(long userId, string email)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
        await orderService.ClaimGuestOrdersAsync(userId, email, null);
    }

    private async Task SendActivationEmailInBackgroundAsync(long userId, string email, string fullName)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        var oldTokens = await db.EmailVerificationTokens
            .Where(t => t.UserId == userId && !t.IsUsed)
            .ToListAsync();
        foreach (var t in oldTokens)
            t.IsUsed = true;

        var token = new EmailVerificationToken
        {
            Token = GenerateSecureToken(),
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };

        db.EmailVerificationTokens.Add(token);
        await db.SaveChangesAsync();

        await emailService.SendActivationEmailAsync(email, fullName, token.Token);
    }

    public async Task SendActivationEmailForUserAsync(User user)
    {
        if (user.Email is null) return;
        await SendActivationEmailInBackgroundAsync(user.Id, user.Email, user.FullName);
    }

    public async Task ResendActivationEmailAsync(long userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is null || user.Email is null) return;
        await SendActivationEmailInBackgroundAsync(user.Id, user.Email, user.FullName);
    }

    public async Task VerifyEmailAsync(string token)
    {
        var verToken = await _db.EmailVerificationTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token);

        if (verToken is null || !verToken.IsValid)
            throw new ArgumentException("Невалідне або протерміноване посилання підтвердження");

        verToken.IsUsed = true;
        verToken.User.IsEmailVerified = true;
        verToken.User.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        _logger.LogInformation("Email verified for user {Email}", verToken.User.Email);
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var storedToken = await _db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (storedToken is null)
            throw new UnauthorizedAccessException("Невалідний refresh token");

        if (storedToken.IsRevoked)
        {
            _logger.LogWarning("Attempt to use revoked refresh token for user {UserId}", storedToken.UserId);
            throw new UnauthorizedAccessException("Refresh token відкликано");
        }

        if (storedToken.IsExpired)
            throw new UnauthorizedAccessException("Refresh token протермінований");

        // Revoke old token
        storedToken.IsRevoked = true;

        // Generate new tokens (rotation)
        var response = await ToAuthResponse(storedToken.User);

        _logger.LogInformation("Refresh token rotated for user {Email}", storedToken.User.Email);

        return response;
    }

    public async Task<AuthResponse> UpdateProfileAsync(long userId, UpdateProfileRequest request)
    {
        var user = await _db.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException("Користувача не знайдено");

        user.FullName = request.FullName;
        user.Phone = request.Phone;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return await ToAuthResponse(user);
    }

    public async Task ChangePasswordAsync(long userId, ChangePasswordRequest request)
    {
        var user = await _db.Users.FindAsync(userId)
            ?? throw new KeyNotFoundException("Користувача не знайдено");

        if (user.PasswordHash is null || !BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Невірний поточний пароль");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        // Revoke all refresh tokens on password change
        var tokens = await _db.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
            token.IsRevoked = true;

        await _db.SaveChangesAsync();

        _logger.LogInformation("User {Email} changed password, {Count} refresh tokens revoked",
            user.Email, tokens.Count);
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        // Always return success to prevent email enumeration
        if (user is null)
        {
            _logger.LogWarning("Password reset requested for non-existent email {Email}", request.Email);
            return;
        }

        // Invalidate previous reset tokens
        var oldTokens = await _db.PasswordResetTokens
            .Where(t => t.UserId == user.Id && !t.IsUsed)
            .ToListAsync();
        foreach (var t in oldTokens)
            t.IsUsed = true;

        var resetToken = new PasswordResetToken
        {
            
            Token = GenerateSecureToken(),
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            CreatedAt = DateTime.UtcNow,
            UserId = user.Id
        };

        _db.PasswordResetTokens.Add(resetToken);
        await _db.SaveChangesAsync();

        await _emailService.SendPasswordResetEmailAsync(user.Email!, user.FullName, resetToken.Token);

        _logger.LogInformation("Password reset token created for user {Email}", user.Email);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        var resetToken = await _db.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == request.Token);

        if (resetToken is null || !resetToken.IsValid)
            throw new ArgumentException("Невалідний або протермінований токен відновлення");

        resetToken.IsUsed = true;
        resetToken.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        resetToken.User.UpdatedAt = DateTime.UtcNow;

        // Revoke all refresh tokens
        var refreshTokens = await _db.RefreshTokens
            .Where(rt => rt.UserId == resetToken.UserId && !rt.IsRevoked)
            .ToListAsync();
        foreach (var rt in refreshTokens)
            rt.IsRevoked = true;

        await _db.SaveChangesAsync();

        _logger.LogInformation("Password reset completed for user {Email}, {Count} refresh tokens revoked",
            resetToken.User.Email, refreshTokens.Count);
    }

    private async Task<AuthResponse> ToAuthResponse(User user)
    {
        var refreshToken = await CreateRefreshToken(user.Id);

        return new AuthResponse
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            IsEmailVerified = user.IsEmailVerified,
            FullName = user.FullName,
            Phone = user.Phone,
            Token = GenerateJwtToken(user),
            RefreshToken = refreshToken.Token
        };
    }

    private async Task<RefreshToken> CreateRefreshToken(long userId)
    {
        var refreshToken = new RefreshToken
        {
            
            Token = GenerateSecureToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpirationInDays),
            CreatedAt = DateTime.UtcNow,
            UserId = userId
        };

        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<AuthResponse> GoogleLoginAsync(GoogleLoginRequest request)
    {
        using var http = new HttpClient();
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.AccessToken);

        var googleUser = await http.GetFromJsonAsync<GoogleUserInfo>("https://www.googleapis.com/oauth2/v3/userinfo")
            ?? throw new UnauthorizedAccessException("Не вдалося отримати дані від Google");

        if (string.IsNullOrEmpty(googleUser.Email))
            throw new UnauthorizedAccessException("Google акаунт не має email");

        var user = await _db.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.GoogleId == googleUser.Sub && !u.IsDeleted);

        if (user is null)
        {
            user = await _db.Users.IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Email == googleUser.Email && !u.IsGuest && !u.IsDeleted);

            if (user is not null)
            {
                user.GoogleId = googleUser.Sub;
                user.IsEmailVerified = true;
                user.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                _logger.LogInformation("Google linked to existing user {Email}", user.Email);
            }
            else
            {
                user = new User
                {
                    Email = googleUser.Email,
                    FullName = googleUser.Name ?? googleUser.Email.Split('@')[0],
                    GoogleId = googleUser.Sub,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _db.Users.Add(user);
                await _db.SaveChangesAsync();
                _logger.LogInformation("New user created via Google: {Email}", user.Email);

                var capturedId = user.Id;
                var capturedEmail = user.Email!;
                var capturedFullName = user.FullName;

                _ = ClaimGuestOrdersInBackgroundAsync(capturedId, capturedEmail)
                    .ContinueWith(t => _logger.LogError(t.Exception, "Failed to claim guest orders for {Email}", capturedEmail),
                        TaskContinuationOptions.OnlyOnFaulted);

                var notifContext = new Dictionary<string, string>
                {
                    ["fullName"] = capturedFullName,
                    ["email"] = capturedEmail,
                    ["phone"] = "",
                    ["registrationDate"] = DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm"),
                };
                _ = _notifications.OnNewUserAsync(capturedId, notifContext)
                    .ContinueWith(t => { }, TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        return await ToAuthResponse(user);
    }

    private class GoogleUserInfo
    {
        [JsonPropertyName("sub")] public string Sub { get; set; } = string.Empty;
        [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("email_verified")] public bool EmailVerified { get; set; }
    }

    private static string GenerateSecureToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.FullName)
        };

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpirationInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
