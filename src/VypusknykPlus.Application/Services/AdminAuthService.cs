using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

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
            const long superAdminId = 0L;
            return new AdminAuthResponse
            {
                Id = superAdminId,
                Email = _superAdminEmail,
                FullName = "Super Admin",
                IsSuperAdmin = true,
                Token = GenerateToken(superAdminId, _superAdminEmail, "Super Admin", null, isSuperAdmin: true),
            };
        }

        // DB admin
        var admin = await _db.Admins
            .Include(a => a.Role)
            .FirstOrDefaultAsync(a => a.Email == request.Email);

        if (admin is null || !BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash))
            throw new UnauthorizedAccessException("Невірний email або пароль");

        admin.LastLoginAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var isSuperAdmin = admin.Role?.IsSuperAdmin ?? false;

        return new AdminAuthResponse
        {
            Id = admin.Id,
            Email = admin.Email,
            FullName = admin.FullName,
            IsSuperAdmin = isSuperAdmin,
            Token = GenerateToken(admin.Id, admin.Email, admin.FullName, admin.Role, isSuperAdmin),
            Role = MapRoleInfo(admin.Role),
        };
    }

    private string GenerateToken(long id, string email, string fullName, Role? role, bool isSuperAdmin)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, id.ToString()),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Name, fullName),
            new(ClaimTypes.Role, "Admin"),
            new("isSuperAdmin", isSuperAdmin.ToString().ToLower()),
        };

        if (role is not null)
        {
            claims.Add(new Claim("roleId", role.Id.ToString()));
            claims.Add(new Claim("roleName", role.Name));
            claims.Add(new Claim("roleColor", role.Color));
            claims.Add(new Claim("pages", JsonSerializer.Serialize(role.Pages)));
        }

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static RoleInfo? MapRoleInfo(Role? r) => r is null ? null : new RoleInfo
    {
        Id = r.Id,
        Name = r.Name,
        Color = r.Color,
        IsSuperAdmin = r.IsSuperAdmin,
        Pages = r.Pages,
    };
}
