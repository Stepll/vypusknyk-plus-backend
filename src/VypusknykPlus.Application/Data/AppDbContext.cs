using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();
    public DbSet<SavedDesign> SavedDesigns => Set<SavedDesign>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<Admin> Admins => Set<Admin>();
    public DbSet<Role> Roles => Set<Role>();

    public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
    public DbSet<ProductSubcategory> ProductSubcategories => Set<ProductSubcategory>();

    public DbSet<StockCategory> StockCategories => Set<StockCategory>();
    public DbSet<StockSubcategory> StockSubcategories => Set<StockSubcategory>();
    public DbSet<StockProduct> StockProducts => Set<StockProduct>();
    public DbSet<StockVariant> StockVariants => Set<StockVariant>();
    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();

    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Delivery> Deliveries => Set<Delivery>();
    public DbSet<DeliveryItem> DeliveryItems => Set<DeliveryItem>();

    public DbSet<DeliveryMethod> DeliveryMethods => Set<DeliveryMethod>();
    public DbSet<InfoPage> InfoPages => Set<InfoPage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global query filter for soft delete
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Order>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<OrderItem>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<SavedDesign>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<CartItem>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Product>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Admin>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Supplier>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Delivery>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Role>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<InfoPage>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<OrderStatus>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<DeliveryMethod>().HasQueryFilter(e => !e.IsDeleted);
    }
}
