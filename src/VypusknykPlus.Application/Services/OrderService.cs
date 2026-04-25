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
        var deliveryMethod = request.Delivery.Method.ToLower() switch
        {
            "nova-poshta" => DeliveryMethod.NovaPoshta,
            "ukrposhta" => DeliveryMethod.Ukrposhta,
            _ => throw new ArgumentException("Невідомий метод доставки")
        };

        var paymentMethod = request.Payment.ToLower() switch
        {
            "cod" => PaymentMethod.Cod,
            "online" => PaymentMethod.Online,
            _ => throw new ArgumentException("Невідомий метод оплати")
        };

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

        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            StatusId = await _db.OrderStatuses
                .Where(s => s.Name == "Прийнято")
                .Select(s => s.Id)
                .FirstAsync(),
            Total = total,
            Delivery = new DeliveryInfo
            {
                Method = deliveryMethod,
                City = request.Delivery.City,
                Warehouse = request.Delivery.Warehouse,
                PostalCode = request.Delivery.PostalCode
            },
            Recipient = new RecipientInfo
            {
                FullName = request.Recipient.FullName,
                Phone = request.Recipient.Phone
            },
            Payment = paymentMethod,
            Email = request.Email,
            Comment = request.Comment,
            UserId = userId,
            IsAnonymous = !userId.HasValue,
            GuestToken = userId.HasValue ? null : request.GuestToken,
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
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        return order is null ? null : MapToResponse(order);
    }

    public async Task<OrderListResponse> GetGuestOrdersAsync(string guestToken)
    {
        var orders = await _db.Orders
            .Include(o => o.Items)
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
            Method = o.Delivery.Method switch
            {
                DeliveryMethod.NovaPoshta => "nova-poshta",
                DeliveryMethod.Ukrposhta => "ukrposhta",
                _ => o.Delivery.Method.ToString()
            },
            City = o.Delivery.City,
            Warehouse = o.Delivery.Warehouse,
            PostalCode = o.Delivery.PostalCode
        },
        Recipient = new OrderRecipientResponse
        {
            FullName = o.Recipient.FullName,
            Phone = o.Recipient.Phone
        },
        Payment = o.Payment switch
        {
            PaymentMethod.Cod => "cod",
            PaymentMethod.Online => "online",
            _ => o.Payment.ToString()
        },
        Comment = o.Comment
    };
}
