using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Auth;
using VypusknykPlus.Application.Entities;
using VypusknykPlus.Application.Services;

namespace VypusknykPlus.Tests.Services;

public class AuthServiceTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new AppDbContext(options);

        var jwtSettings = Options.Create(new JwtSettings
        {
            Key = "super-secret-test-key-that-is-long-enough-for-hmacsha256",
            Issuer = "test-issuer",
            Audience = "test-audience",
            ExpirationInMinutes = 15,
            RefreshTokenExpirationInDays = 30
        });

        _sut = new AuthService(
            _db,
            jwtSettings,
            new Mock<IEmailService>().Object,
            new Mock<IOrderService>().Object,
            new Mock<INotificationService>().Object,
            new Mock<ITaskService>().Object,
            new Mock<IServiceScopeFactory>().Object,
            new Mock<ILogger<AuthService>>().Object
        );
    }

    public void Dispose() => _db.Dispose();

    // ── Login ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsAuthResponseWithTokens()
    {
        var password = "Password123!";
        var user = CreateUser("login@test.com", password);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var result = await _sut.LoginAsync(new LoginRequest { Email = user.Email, Password = password });

        Assert.Equal(user.Email, result.Email);
        Assert.NotEmpty(result.Token);
        Assert.NotEmpty(result.RefreshToken);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ThrowsUnauthorizedAccessException()
    {
        var user = CreateUser("login2@test.com", "CorrectPassword123!");
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _sut.LoginAsync(new LoginRequest { Email = user.Email, Password = "WrongPassword" }));
    }

    [Fact]
    public async Task LoginAsync_UnknownEmail_ThrowsUnauthorizedAccessException()
    {
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _sut.LoginAsync(new LoginRequest { Email = "nobody@test.com", Password = "Password123!" }));
    }

    // ── Register ──────────────────────────────────────────────────────────────

    [Fact]
    public async Task RegisterAsync_NewEmail_SavesUserAndReturnsAuthResponse()
    {
        var request = new RegisterRequest
        {
            Email = "new@test.com",
            Password = "Password123!",
            FullName = "Іван Франко",
            Phone = "+380501234567"
        };

        var result = await _sut.RegisterAsync(request);

        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.FullName, result.FullName);
        Assert.NotEmpty(result.Token);
        Assert.True(await _db.Users.AnyAsync(u => u.Email == request.Email));
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ThrowsArgumentException()
    {
        var user = CreateUser("dup@test.com", "Password123!");
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.RegisterAsync(new RegisterRequest
            {
                Email = user.Email,
                Password = "Password123!",
                FullName = "Duplicate"
            }));
    }

    // ── Refresh token ─────────────────────────────────────────────────────────

    [Fact]
    public async Task RefreshTokenAsync_ValidToken_RevokesOldAndReturnsNewTokens()
    {
        var user = CreateUser("refresh@test.com", "Password123!");
        _db.Users.Add(user);

        var oldToken = new RefreshToken
        {
            Token = "valid-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow,
            UserId = user.Id,
            User = user
        };
        _db.RefreshTokens.Add(oldToken);
        await _db.SaveChangesAsync();

        var result = await _sut.RefreshTokenAsync(new RefreshTokenRequest { RefreshToken = "valid-refresh-token" });

        Assert.NotEmpty(result.Token);
        Assert.NotEqual("valid-refresh-token", result.RefreshToken);
        Assert.True((await _db.RefreshTokens.FindAsync(oldToken.Id))!.IsRevoked);
    }

    [Fact]
    public async Task RefreshTokenAsync_RevokedToken_ThrowsUnauthorizedAccessException()
    {
        var user = CreateUser("revoked@test.com", "Password123!");
        _db.Users.Add(user);
        _db.RefreshTokens.Add(new RefreshToken
        {
            Token = "revoked-token",
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = true,
            UserId = user.Id,
            User = user
        });
        await _db.SaveChangesAsync();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _sut.RefreshTokenAsync(new RefreshTokenRequest { RefreshToken = "revoked-token" }));
    }

    // ── ChangePassword ────────────────────────────────────────────────────────

    [Fact]
    public async Task ChangePasswordAsync_ValidCurrentPassword_RevokesAllRefreshTokens()
    {
        const string currentPassword = "OldPassword123!";
        var user = CreateUser("changepass@test.com", currentPassword);
        _db.Users.Add(user);

        _db.RefreshTokens.AddRange(Enumerable.Range(0, 3).Select(_ => new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow,
            UserId = user.Id
        }));
        await _db.SaveChangesAsync();

        await _sut.ChangePasswordAsync(user.Id, new ChangePasswordRequest
        {
            CurrentPassword = currentPassword,
            NewPassword = "NewPassword123!"
        });

        var allRevoked = await _db.RefreshTokens
            .Where(rt => rt.UserId == user.Id)
            .AllAsync(rt => rt.IsRevoked);
        Assert.True(allRevoked);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static User CreateUser(string email, string password, long id = 0) => new()
    {
        Id = id,
        Email = email,
        FullName = "Тестовий Користувач",
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
}
