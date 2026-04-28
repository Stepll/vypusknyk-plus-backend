using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
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
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext db, IOptions<JwtSettings> jwt, IEmailService emailService, IOrderService orderService, ILogger<AuthService> logger)
    {
        _db = db;
        _jwt = jwt.Value;
        _emailService = emailService;
        _orderService = orderService;
        _logger = logger;
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsGuest);

        if (user is null || user.PasswordHash is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Невірний email або пароль");

        _logger.LogInformation("User {Email} logged in", user.Email);

        _ = _orderService.ClaimGuestOrdersAsync(user.Id, user.Email!, null)
            .ContinueWith(t => _logger.LogError(t.Exception, "Failed to claim guest orders for {Email}", user.Email),
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

        _ = _orderService.ClaimGuestOrdersAsync(user.Id, user.Email!, null)
            .ContinueWith(t => _logger.LogError(t.Exception, "Failed to claim guest orders for {Email}", user.Email),
                TaskContinuationOptions.OnlyOnFaulted);

        return await ToAuthResponse(user);
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
