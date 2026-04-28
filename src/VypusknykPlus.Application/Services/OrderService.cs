using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Orders;
using VypusknykPlus.Application.Entities;
using VypusknykPlus.Application.ValueObjects;

namespace VypusknykPlus.Application.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;
    private readonly IEmailService _email;
    private readonly ILogger<OrderService> _logger;

    public OrderService(AppDbContext db, IEmailService email, ILogger<OrderService> logger)
    {
        _db = db;
        _email = email;
        _logger = logger;
    }

    public async Task<OrderResponse> CreateAsync(long? userId, CreateOrderRequest request)
    {
        var deliveryMethod = await _db.DeliveryMethods
            .FirstOrDefaultAsync(dm => dm.Slug == request.Delivery.Method.ToLower() && dm.IsEnabled)
            ?? throw new ArgumentException("Невідомий або неактивний метод доставки");

        var paymentMethod = await _db.PaymentMethods
            .FirstOrDefaultAsync(pm => pm.Slug == request.Payment.ToLower() && pm.IsEnabled)
            ?? throw new ArgumentException("Невідомий або неактивний метод оплати");

        var productIds = request.Items
            .Where(i => i.ProductId.HasValue)
            .Select(i => (long)i.ProductId!.Value)
            .Distinct()
            .ToList();

        var productCategories = productIds.Count > 0
            ? await _db.Products
                .IgnoreQueryFilters()
                .Include(p => p.Category)
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.Category.Name)
            : new Dictionary<long, string>();

        var orderItems = request.Items.Select(i => new OrderItem
        {
            Name = i.Name,
            Quantity = i.Qty,
            Price = i.Price,
            NamesData = i.NamesData,
            RibbonCustomization = i.RibbonCustomization,
            ProductCategory = i.ProductId.HasValue && productCategories.TryGetValue(i.ProductId.Value, out var cat) ? cat : null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }).ToList();

        var total = orderItems.Sum(i => i.Quantity * i.Price);

        if (!userId.HasValue && !string.IsNullOrWhiteSpace(request.Recipient.Phone))
            userId = await FindOrCreateGuestUserAsync(request.Recipient.Phone, request.Recipient.FullName);

        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            StatusId = await _db.OrderStatuses
                .Where(s => s.Name == "Прийнято")
                .Select(s => s.Id)
                .FirstAsync(),
            Total = total,
            DeliveryMethodId = deliveryMethod.Id,
            DeliveryMethod = deliveryMethod,
            Delivery = new DeliveryInfo
            {
                City = request.Delivery.City,
                Warehouse = request.Delivery.Warehouse,
                PostalCode = request.Delivery.PostalCode
            },
            Recipient = new RecipientInfo
            {
                FullName = request.Recipient.FullName,
                Phone = request.Recipient.Phone
            },
            PaymentMethodId = paymentMethod.Id,
            PaymentMethod = paymentMethod,
            Email = request.Email,
            Comment = request.Comment,
            UserId = userId,
            IsAnonymous = false,
            GuestToken = null,
            Items = orderItems,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Order {OrderNumber} created for user {UserId}, total {Total}",
            order.OrderNumber, userId, total);

        var toEmail = request.Email;
        if (string.IsNullOrEmpty(toEmail) && userId.HasValue)
        {
            var user = await _db.Users.FindAsync(userId.Value);
            toEmail = user?.Email;
        }

        if (!string.IsNullOrEmpty(toEmail))
        {
            _ = _email.SendOrderConfirmationEmailAsync(toEmail, request.Recipient.FullName, order.OrderNumber, total)
                .ContinueWith(t => _logger.LogError(t.Exception, "Failed to send order confirmation email for {OrderNumber}", order.OrderNumber),
                    TaskContinuationOptions.OnlyOnFaulted);
        }

        return MapToResponse(order);
    }

    public async Task<OrderListResponse> GetUserOrdersAsync(long userId)
    {
        var orders = await _db.Orders
            .Include(o => o.Items)
            .Include(o => o.DeliveryMethod)
            .Include(o => o.PaymentMethod)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return new OrderListResponse
        {
            Items = orders.Select(MapToResponse).ToList()
        };
    }

    public async Task<OrderResponse?> GetByIdAsync(long userId, long orderId)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .Include(o => o.DeliveryMethod)
            .Include(o => o.PaymentMethod)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        return order is null ? null : MapToResponse(order);
    }

    public async Task<OrderListResponse> GetGuestOrdersAsync(string guestToken)
    {
        var orders = await _db.Orders
            .Include(o => o.Items)
            .Include(o => o.DeliveryMethod)
            .Include(o => o.PaymentMethod)
            .Where(o => o.GuestToken == guestToken)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return new OrderListResponse { Items = orders.Select(MapToResponse).ToList() };
    }

    public async Task ClaimGuestOrdersAsync(long userId, string userEmail, string? guestToken)
    {
        var orders = await _db.Orders
            .Where(o => o.UserId == null &&
                (o.GuestToken == guestToken || o.Email == userEmail))
            .ToListAsync();

        foreach (var order in orders)
        {
            order.UserId = userId;
            order.IsAnonymous = true;
        }

        if (orders.Count > 0)
        {
            await _db.SaveChangesAsync();
            _logger.LogInformation("Claimed {Count} guest orders for user {UserId}", orders.Count, userId);
        }
    }

    private async Task<long> FindOrCreateGuestUserAsync(string phone, string fullName)
    {
        var existing = await _db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Phone == phone && !u.IsDeleted);

        if (existing is not null)
            return existing.Id;

        var guest = new User
        {
            IsGuest = true,
            FullName = fullName,
            Phone = phone,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Users.Add(guest);
        await _db.SaveChangesAsync();
        return guest.Id;
    }

    private static string GenerateOrderNumber()
    {
        return "VIP-" + Guid.NewGuid().ToString("N")[..6].ToUpper();
    }

    private static OrderResponse MapToResponse(Order o) => new()
    {
        Id = o.Id,
        OrderNumber = o.OrderNumber,
        Date = o.CreatedAt,
        Status = o.OrderStatus?.Name ?? string.Empty,
        Items = o.Items.Select(i => new OrderItemResponse
        {
            Name = i.Name,
            Qty = i.Quantity,
            Price = i.Price
        }).ToList(),
        Total = o.Total,
        Delivery = new OrderDeliveryResponse
        {
            Method = o.DeliveryMethod?.Slug ?? string.Empty,
            City = o.Delivery.City,
            Warehouse = o.Delivery.Warehouse,
            PostalCode = o.Delivery.PostalCode
        },
        Recipient = new OrderRecipientResponse
        {
            FullName = o.Recipient.FullName,
            Phone = o.Recipient.Phone
        },
        Payment = o.PaymentMethod?.Slug ?? string.Empty,
        Comment = o.Comment
    };
}
