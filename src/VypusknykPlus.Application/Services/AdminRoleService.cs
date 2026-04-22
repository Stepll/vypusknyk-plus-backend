using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Services;

public class AdminRoleService : IAdminRoleService
{
    private readonly AppDbContext _db;

    public AdminRoleService(AppDbContext db) => _db = db;

    public async Task<List<RoleResponse>> GetRolesAsync()
    {
        return await _db.Roles
            .AsNoTracking()
            .OrderBy(r => r.Id)
            .Select(r => MapRole(r))
            .ToListAsync();
    }

    public async Task<RoleResponse?> GetRoleAsync(long id)
    {
        var role = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        return role is null ? null : MapRole(role);
    }

    public async Task<RoleResponse> CreateRoleAsync(CreateRoleRequest request)
    {
        var role = new Role
        {
            Name = request.Name,
            Color = request.Color,
            Pages = request.Pages,
            IsSuperAdmin = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _db.Roles.Add(role);
        await _db.SaveChangesAsync();
        return MapRole(role);
    }

    public async Task<RoleResponse> UpdateRoleAsync(long id, UpdateRoleRequest request)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new KeyNotFoundException($"Роль {id} не знайдено");

        if (role.IsSuperAdmin)
            throw new InvalidOperationException("Системну роль SuperAdmin не можна змінювати");

        role.Name = request.Name;
        role.Color = request.Color;
        role.Pages = request.Pages;
        role.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return MapRole(role);
    }

    public async Task DeleteRoleAsync(long id)
    {
        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new KeyNotFoundException($"Роль {id} не знайдено");

        if (role.IsSuperAdmin)
            throw new InvalidOperationException("Системну роль SuperAdmin не можна видаляти");

        role.IsDeleted = true;
        role.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    private static RoleResponse MapRole(Role r) => new()
    {
        Id = r.Id,
        Name = r.Name,
        Color = r.Color,
        Pages = r.Pages,
        IsSuperAdmin = r.IsSuperAdmin,
        CreatedAt = r.CreatedAt,
    };
}
