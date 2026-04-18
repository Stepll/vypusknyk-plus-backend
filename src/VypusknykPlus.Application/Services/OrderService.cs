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

    public async Task<OrderResponse> CreateAsync(Guid? userId, CreateOrderRequest request)
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

        var orderItems = request.Items.Select(i => new OrderItem
        {
            Id = Guid.NewGuid(),
            Name = i.Name,
            Quantity = i.Qty,
            Price = i.Price,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        }).ToList();

        var total = orderItems.Sum(i => i.Quantity * i.Price);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = GenerateOrderNumber(),
            Status = OrderStatus.Accepted,
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

    public async Task<OrderListResponse> GetUserOrdersAsync(Guid userId)
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

    public async Task<OrderResponse?> GetByIdAsync(Guid userId, Guid orderId)
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

    public async Task ClaimGuestOrdersAsync(Guid userId, string userEmail, string? guestToken)
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
        Status = o.Status.ToString().ToLower(),
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
