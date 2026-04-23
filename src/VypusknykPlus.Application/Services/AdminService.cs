using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs;
using VypusknykPlus.Application.DTOs.Admin;
using VypusknykPlus.Application.Entities;
using Role = VypusknykPlus.Application.Entities.Role;

namespace VypusknykPlus.Application.Services;

public class AdminService : IAdminService
{
    private readonly AppDbContext _db;
    private readonly IImageService _imageService;

    public AdminService(AppDbContext db, IImageService imageService)
    {
        _db = db;
        _imageService = imageService;
    }

    public async Task<PagedResponse<AdminOrderResponse>> GetOrdersAsync(int page, int pageSize, string? status)
    {
        var query = _db.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<OrderStatus>(status, true, out var parsed))
            query = query.Where(o => o.Status == parsed);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<AdminOrderResponse>
        {
            Items = items.Select(MapOrder).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task<AdminOrderResponse?> GetOrderAsync(long id)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);

        return order is null ? null : MapOrder(order);
    }

    public async Task UpdateOrderStatusAsync(long id, string status)
    {
        if (!Enum.TryParse<OrderStatus>(status, true, out var parsed))
            throw new ArgumentException($"Невідомий статус: {status}");

        var order = await _db.Orders.FindAsync(id)
            ?? throw new KeyNotFoundException($"Замовлення {id} не знайдено");

        order.Status = parsed;
        order.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<PagedResponse<AdminProductResponse>> GetProductsAsync(int page, int pageSize)
    {
        var query = _db.Products.IgnoreQueryFilters().AsNoTracking();

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<AdminProductResponse>
        {
            Items = items.Select(p => new AdminProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Category = p.Category.ToString(),
                ImageUrl = _imageService.GetPublicUrl(p.ImageKey),
                IsDeleted = p.IsDeleted,
            }).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task<AdminProductDetailResponse?> GetProductAsync(long id)
    {
        var p = await _db.Products
            .IgnoreQueryFilters()
            .Include(p => p.Images.OrderBy(i => i.CreatedAt))
            .FirstOrDefaultAsync(p => p.Id == id);
        return p is null ? null : MapProductDetail(p);
    }

    public async Task<AdminProductDetailResponse> CreateProductAsync(SaveProductRequest request)
    {
        if (!Enum.TryParse<ProductCategory>(request.Category, true, out var category))
            throw new ArgumentException($"Невідома категорія: {request.Category}");

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            MinOrder = request.MinOrder,
            Category = category,
            Color = string.IsNullOrWhiteSpace(request.Color) ? null : request.Color,
            Tags = request.Tags,
            Popular = request.Popular,
            IsNew = request.IsNew,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return MapProductDetail(product);
    }

    public async Task<AdminProductDetailResponse> UpdateProductAsync(long id, SaveProductRequest request)
    {
        var product = await _db.Products.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new KeyNotFoundException($"Продукт {id} не знайдено");

        if (!Enum.TryParse<ProductCategory>(request.Category, true, out var category))
            throw new ArgumentException($"Невідома категорія: {request.Category}");

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.MinOrder = request.MinOrder;
        product.Category = category;
        product.Color = string.IsNullOrWhiteSpace(request.Color) ? null : request.Color;
        product.Tags = request.Tags;
        product.Popular = request.Popular;
        product.IsNew = request.IsNew;
        product.IsDeleted = request.IsDeleted;
        product.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return MapProductDetail(product);
    }

    public async Task DeleteProductAsync(long id)
    {
        var product = await _db.Products.FindAsync(id)
            ?? throw new KeyNotFoundException($"Продукт {id} не знайдено");

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<AdminProductDetailResponse> UploadProductImageAsync(long productId, Stream stream, string contentType)
    {
        var product = await _db.Products
            .IgnoreQueryFilters()
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId)
            ?? throw new KeyNotFoundException($"Продукт {productId} не знайдено");

        var extension = contentType switch
        {
            "image/jpeg" => "jpg",
            "image/png"  => "png",
            "image/webp" => "webp",
            _            => "jpg"
        };
        var objectKey = $"products/{productId}/{Guid.NewGuid():N}.{extension}";
        await _imageService.UploadAsync(objectKey, stream, contentType);

        var isFirst = !product.Images.Any();
        var image = new ProductImage
        {
            ProductId = productId,
            ImageKey = objectKey,
            IsPreview = isFirst,
            CreatedAt = DateTime.UtcNow,
        };
        _db.ProductImages.Add(image);

        if (isFirst)
        {
            product.ImageKey = objectKey;
            product.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        return await LoadProductDetailAsync(productId);
    }

    public async Task<AdminProductDetailResponse> DeleteProductImageAsync(long productId, long imageId)
    {
        var product = await _db.Products
            .IgnoreQueryFilters()
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId)
            ?? throw new KeyNotFoundException($"Продукт {productId} не знайдено");

        var image = product.Images.FirstOrDefault(i => i.Id == imageId)
            ?? throw new KeyNotFoundException($"Зображення {imageId} не знайдено");

        await _imageService.DeleteAsync(image.ImageKey);
        _db.ProductImages.Remove(image);

        if (image.IsPreview)
        {
            var next = product.Images
                .Where(i => i.Id != imageId)
                .OrderBy(i => i.CreatedAt)
                .FirstOrDefault();

            if (next is not null)
            {
                next.IsPreview = true;
                product.ImageKey = next.ImageKey;
            }
            else
            {
                product.ImageKey = null;
            }
            product.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        return await LoadProductDetailAsync(productId);
    }

    public async Task<AdminProductDetailResponse> SetPreviewImageAsync(long productId, long imageId)
    {
        var product = await _db.Products
            .IgnoreQueryFilters()
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId)
            ?? throw new KeyNotFoundException($"Продукт {productId} не знайдено");

        var target = product.Images.FirstOrDefault(i => i.Id == imageId)
            ?? throw new KeyNotFoundException($"Зображення {imageId} не знайдено");

        foreach (var img in product.Images)
            img.IsPreview = img.Id == imageId;

        product.ImageKey = target.ImageKey;
        product.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return await LoadProductDetailAsync(productId);
    }

    private async Task<AdminProductDetailResponse> LoadProductDetailAsync(long productId)
    {
        var p = await _db.Products
            .IgnoreQueryFilters()
            .Include(p => p.Images.OrderBy(i => i.CreatedAt))
            .FirstAsync(p => p.Id == productId);
        return MapProductDetail(p);
    }

    public async Task<PagedResponse<AdminUserResponse>> GetUsersAsync(int page, int pageSize)
    {
        var total = await _db.Users.CountAsync();
        var items = await _db.Users
            .AsNoTracking()
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new AdminUserResponse
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                CreatedAt = u.CreatedAt,
                OrdersCount = u.Orders.Count,
            })
            .ToListAsync();

        return new PagedResponse<AdminUserResponse>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task<AdminUserDetailResponse?> GetUserAsync(long id)
    {
        var user = await _db.Users
            .AsNoTracking()
            .Include(u => u.Orders)
                .ThenInclude(o => o.Items)
            .Include(u => u.SavedDesigns)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null) return null;

        return new AdminUserDetailResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Phone = user.Phone,
            CreatedAt = user.CreatedAt,
            Orders = user.Orders
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new AdminUserOrderSummary
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    Status = o.Status.ToString(),
                    Total = o.Total,
                    ItemsCount = o.Items.Count,
                    CreatedAt = o.CreatedAt,
                }).ToList(),
            SavedDesigns = user.SavedDesigns
                .Where(d => !d.IsDeleted)
                .OrderByDescending(d => d.SavedAt)
                .Select(d => new AdminUserSavedDesign
                {
                    Id = d.Id,
                    DesignName = d.DesignName,
                    SavedAt = d.SavedAt,
                }).ToList(),
        };
    }

    public async Task<AdminAdminDetailResponse?> GetAdminDetailAsync(long id)
    {
        var admin = await _db.Admins
            .IgnoreQueryFilters()
            .Include(a => a.Role)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        return admin is null ? null : new AdminAdminDetailResponse
        {
            Id = admin.Id,
            Email = admin.Email,
            FullName = admin.FullName,
            CreatedAt = admin.CreatedAt,
            LastLoginAt = admin.LastLoginAt,
            Role = MapRoleInfo(admin.Role),
        };
    }

    public async Task<AdminAdminDetailResponse> CreateAdminAsync(CreateAdminRequest request)
    {
        var admin = new Entities.Admin
        {
            Email = request.Email,
            FullName = request.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            RoleId = request.RoleId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _db.Admins.Add(admin);
        await _db.SaveChangesAsync();

        var role = request.RoleId.HasValue
            ? await _db.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == request.RoleId)
            : null;

        return new AdminAdminDetailResponse
        {
            Id = admin.Id,
            Email = admin.Email,
            FullName = admin.FullName,
            CreatedAt = admin.CreatedAt,
            LastLoginAt = null,
            Role = MapRoleInfo(role),
        };
    }

    public async Task DeleteAdminAsync(long id)
    {
        var admin = await _db.Admins.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Id == id)
            ?? throw new KeyNotFoundException($"Адміна {id} не знайдено");

        admin.IsDeleted = true;
        admin.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task ChangeAdminPasswordAsync(long id, string newPassword)
    {
        var admin = await _db.Admins.IgnoreQueryFilters().FirstOrDefaultAsync(a => a.Id == id)
            ?? throw new KeyNotFoundException($"Адміна {id} не знайдено");

        admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        admin.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<AdminAdminDetailResponse> ChangeAdminRoleAsync(long id, long? roleId)
    {
        var admin = await _db.Admins
            .IgnoreQueryFilters()
            .Include(a => a.Role)
            .FirstOrDefaultAsync(a => a.Id == id)
            ?? throw new KeyNotFoundException($"Адміна {id} не знайдено");

        admin.RoleId = roleId;
        admin.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var role = roleId.HasValue
            ? await _db.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.Id == roleId)
            : null;

        return new AdminAdminDetailResponse
        {
            Id = admin.Id,
            Email = admin.Email,
            FullName = admin.FullName,
            CreatedAt = admin.CreatedAt,
            LastLoginAt = admin.LastLoginAt,
            Role = MapRoleInfo(role),
        };
    }

    public async Task<PagedResponse<AdminSavedDesignResponse>> GetSavedDesignsAsync(int page, int pageSize)
    {
        var query = _db.SavedDesigns
            .IgnoreQueryFilters()
            .Include(d => d.User)
            .Where(d => !d.IsDeleted)
            .AsNoTracking();

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(d => d.SavedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new AdminSavedDesignResponse
            {
                Id = d.Id,
                DesignName = d.DesignName,
                SavedAt = d.SavedAt,
                UserId = d.UserId,
                UserFullName = d.User.FullName,
                UserEmail = d.User.Email,
                State = d.State,
            })
            .ToListAsync();

        return new PagedResponse<AdminSavedDesignResponse>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize,
        };
    }

    public async Task<AdminSavedDesignDetailResponse?> GetSavedDesignAsync(long id)
    {
        var d = await _db.SavedDesigns
            .IgnoreQueryFilters()
            .Include(d => d.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

        if (d is null) return null;

        return new AdminSavedDesignDetailResponse
        {
            Id = d.Id,
            DesignName = d.DesignName,
            SavedAt = d.SavedAt,
            UserId = d.UserId,
            UserFullName = d.User.FullName,
            UserEmail = d.User.Email,
            State = d.State,
        };
    }

    public async Task DeleteSavedDesignAsync(long id)
    {
        var design = await _db.SavedDesigns
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);

        if (design is null)
            throw new KeyNotFoundException("Дизайн не знайдено");

        design.IsDeleted = true;
        design.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<PagedResponse<AdminAdminResponse>> GetAdminsAsync(int page, int pageSize)
    {
        var query = _db.Admins.IgnoreQueryFilters().Include(a => a.Role).AsNoTracking();

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(a => a.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<AdminAdminResponse>
        {
            Items = items.Select(a => new AdminAdminResponse
            {
                Id = a.Id,
                Email = a.Email,
                FullName = a.FullName,
                CreatedAt = a.CreatedAt,
                Role = MapRoleInfo(a.Role),
            }).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize,
        };
    }

    private AdminProductDetailResponse MapProductDetail(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Price = p.Price,
        MinOrder = p.MinOrder,
        Category = p.Category.ToString(),
        Color = p.Color,
        Tags = p.Tags,
        Popular = p.Popular,
        IsNew = p.IsNew,
        ImageUrl = _imageService.GetPublicUrl(p.ImageKey),
        IsDeleted = p.IsDeleted,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt,
        Images = p.Images.Select(i => new ProductImageResponse
        {
            Id = i.Id,
            ImageUrl = _imageService.GetPublicUrl(i.ImageKey)!,
            IsPreview = i.IsPreview,
        }).ToList(),
    };

    private static RoleInfo? MapRoleInfo(Role? r) => r is null ? null : new RoleInfo
    {
        Id = r.Id,
        Name = r.Name,
        Color = r.Color,
        IsSuperAdmin = r.IsSuperAdmin,
        Pages = r.Pages,
    };

    private static AdminOrderResponse MapOrder(Order o) => new()
    {
        Id = o.Id,
        OrderNumber = o.OrderNumber,
        CreatedAt = o.CreatedAt,
        Status = o.Status.ToString(),
        Total = o.Total,
        IsAnonymous = o.IsAnonymous,
        UserId = o.UserId,
        Email = o.Email,
        Comment = o.Comment,
        Payment = o.Payment.ToString(),
        Recipient = new AdminRecipientResponse
        {
            FullName = o.Recipient.FullName,
            Phone = o.Recipient.Phone,
        },
        Delivery = new AdminDeliveryResponse
        {
            Method = o.Delivery.Method.ToString(),
            City = o.Delivery.City,
            Warehouse = o.Delivery.Warehouse,
        },
        Items = o.Items.Select(i => new AdminOrderItemResponse
        {
            Id = i.Id,
            Name = i.Name,
            Quantity = i.Quantity,
            Price = i.Price,
            NamesData = i.NamesData,
            RibbonCustomization = i.RibbonCustomization,
        }).ToList(),
    };
}
