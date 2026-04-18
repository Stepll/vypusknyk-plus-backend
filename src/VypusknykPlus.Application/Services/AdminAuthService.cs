using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;

namespace VypusknykPlus.Application.Services;

public class AdminAuthService : IAdminAuthService
{
    private readonly AppDbContext _db;
    private readonly JwtSettings _jwt;
    private readonly string? _superAdminEmail;
    private readonly string? _superAdminPassword;

    public AdminAuthService(AppDbContext db, IOptions<JwtSettings> jwt, IConfiguration config)
    {
        _db = db;
        _jwt = jwt.Value;
        _superAdminEmail = config["Admin:Email"];
        _superAdminPassword = config["Admin:Password"];
    }

    public async Task<AdminAuthResponse> LoginAsync(AdminLoginRequest request)
    {
        // Super admin from env vars
        if (!string.IsNullOrWhiteSpace(_superAdminEmail) &&
            string.Equals(request.Email, _superAdminEmail, StringComparison.OrdinalIgnoreCase) &&
            !string.IsNullOrWhiteSpace(_superAdminPassword) &&
            request.Password == _superAdminPassword)
        {
            var superAdminId = Guid.Empty;
            return new AdminAuthResponse
            {
                Id = superAdminId,
                Email = _superAdminEmail,
                FullName = "Super Admin",
                IsSuperAdmin = true,
                Token = GenerateToken(superAdminId, _superAdminEmail, "Super Admin"),
            };
        }

        // DB admin
        var admin = await _db.Admins.FirstOrDefaultAsync(a => a.Email == request.Email);

        if (admin is null || !BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash))
            throw new UnauthorizedAccessException("Невірний email або пароль");

        return new AdminAuthResponse
        {
            Id = admin.Id,
            Email = admin.Email,
            FullName = admin.FullName,
            IsSuperAdmin = false,
            Token = GenerateToken(admin.Id, admin.Email, admin.FullName),
        };
    }

    private string GenerateToken(Guid id, string email, string fullName)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, fullName),
            new Claim(ClaimTypes.Role, "Admin"),
        };

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
