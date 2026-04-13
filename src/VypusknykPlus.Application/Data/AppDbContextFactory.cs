using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VypusknykPlus.Application.Data;

/// <summary>
/// Design-time factory used by EF Core CLI tools (migrations add, database update, etc.).
/// Bypasses the application startup so that MinIO/DB connections are not required
/// when running dotnet ef commands locally.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=vypusknyk_plus;Username=postgres;Password=postgres");
        return new AppDbContext(optionsBuilder.Options);
    }
}
