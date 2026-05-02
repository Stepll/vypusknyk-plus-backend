using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Promotions;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Services;

public class PromotionService(AppDbContext db) : IPromotionService
{
    // ─── Admin — Promotions ────────────────────────────────────────────────────

    public async Task<List<AdminPromotionResponse>> GetAdminPromotionsAsync()
    {
        var items = await db.Promotions
            .IgnoreQueryFilters()
            .Where(p => !p.IsDeleted)
            .Include(p => p.Category)
            .Include(p => p.Subcategory)
            .Include(p => p.Product)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return items.Select(MapAdminPromotion).ToList();
    }

    public async Task<AdminPromotionResponse> CreatePromotionAsync(SavePromotionRequest request)
    {
        var entity = new Promotion
        {
            Name = request.Name,
            Description = request.Description,
            DiscountType = ParseDiscountType(request.DiscountType),
            DiscountValue = request.DiscountValue,
            Scope = ParseScope(request.Scope),
            CategoryId = request.CategoryId,
            SubcategoryId = request.SubcategoryId,
            ProductId = request.ProductId,
            MinOrderAmount = request.MinOrderAmount,
            StartsAt = request.StartsAt,
            EndsAt = request.EndsAt,
            IsActive = request.IsActive,
            IsOneTimePerUser = request.IsOneTimePerUser,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.Promotions.Add(entity);
        await db.SaveChangesAsync();

        await db.Entry(entity).Reference(p => p.Category).LoadAsync();
        await db.Entry(entity).Reference(p => p.Subcategory).LoadAsync();
        await db.Entry(entity).Reference(p => p.Product).LoadAsync();

        return MapAdminPromotion(entity);
    }

    public async Task<AdminPromotionResponse> UpdatePromotionAsync(long id, SavePromotionRequest request)
    {
        var entity = await db.Promotions
            .IgnoreQueryFilters()
            .Include(p => p.Category)
            .Include(p => p.Subcategory)
            .Include(p => p.Product)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted)
            ?? throw new KeyNotFoundException($"Promotion {id} not found");

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.DiscountType = ParseDiscountType(request.DiscountType);
        entity.DiscountValue = request.DiscountValue;
        entity.Scope = ParseScope(request.Scope);
        entity.CategoryId = request.CategoryId;
        entity.SubcategoryId = request.SubcategoryId;
        entity.ProductId = request.ProductId;
        entity.MinOrderAmount = request.MinOrderAmount;
        entity.StartsAt = request.StartsAt;
        entity.EndsAt = request.EndsAt;
        entity.IsActive = request.IsActive;
        entity.IsOneTimePerUser = request.IsOneTimePerUser;
        entity.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return MapAdminPromotion(entity);
    }

    public async Task DeletePromotionAsync(long id)
    {
        var entity = await db.Promotions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted)
            ?? throw new KeyNotFoundException($"Promotion {id} not found");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    // ─── Admin — Promo codes ───────────────────────────────────────────────────

    public async Task<List<AdminPromoCodeResponse>> GetAdminPromoCodesAsync()
    {
        var items = await db.PromoCodes
            .IgnoreQueryFilters()
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return items.Select(MapAdminPromoCode).ToList();
    }

    public async Task<AdminPromoCodeResponse> CreatePromoCodeAsync(SavePromoCodeRequest request)
    {
        var normalized = request.Code.Trim().ToUpper();
        if (await db.PromoCodes.IgnoreQueryFilters().AnyAsync(p => p.Code == normalized && !p.IsDeleted))
            throw new InvalidOperationException($"Промокод '{normalized}' вже існує");

        var entity = new PromoCode
        {
            Code = normalized,
            DisplayName = request.DisplayName,
            CardColor = request.CardColor,
            Description = request.Description,
            DiscountType = ParseDiscountType(request.DiscountType),
            DiscountValue = request.DiscountValue,
            MinOrderAmount = request.MinOrderAmount,
            MaxUsages = request.MaxUsages,
            IsOneTimePerUser = request.IsOneTimePerUser,
            StartsAt = request.StartsAt,
            EndsAt = request.EndsAt,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.PromoCodes.Add(entity);
        await db.SaveChangesAsync();
        return MapAdminPromoCode(entity);
    }

    public async Task<AdminPromoCodeResponse> UpdatePromoCodeAsync(long id, SavePromoCodeRequest request)
    {
        var entity = await db.PromoCodes
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted)
            ?? throw new KeyNotFoundException($"PromoCode {id} not found");

        var normalized = request.Code.Trim().ToUpper();
        if (await db.PromoCodes.IgnoreQueryFilters().AnyAsync(p => p.Code == normalized && p.Id != id && !p.IsDeleted))
            throw new InvalidOperationException($"Промокод '{normalized}' вже існує");

        entity.Code = normalized;
        entity.DisplayName = request.DisplayName;
        entity.CardColor = request.CardColor;
        entity.Description = request.Description;
        entity.DiscountType = ParseDiscountType(request.DiscountType);
        entity.DiscountValue = request.DiscountValue;
        entity.MinOrderAmount = request.MinOrderAmount;
        entity.MaxUsages = request.MaxUsages;
        entity.IsOneTimePerUser = request.IsOneTimePerUser;
        entity.StartsAt = request.StartsAt;
        entity.EndsAt = request.EndsAt;
        entity.IsActive = request.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return MapAdminPromoCode(entity);
    }

    public async Task DeletePromoCodeAsync(long id)
    {
        var entity = await db.PromoCodes
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted)
            ?? throw new KeyNotFoundException($"PromoCode {id} not found");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    // ─── Public ────────────────────────────────────────────────────────────────

    public async Task<List<PublicPromotionResponse>> GetPublicPromotionsAsync()
    {
        var now = DateTime.UtcNow;
        var items = await db.Promotions
            .Where(p => p.IsActive && (p.EndsAt == null || p.EndsAt >= now))
            .OrderBy(p => p.StartsAt)
            .ToListAsync();

        return items.Select(MapPublicPromotion).ToList();
    }

    public async Task<List<PromoCodeCardResponse>> GetMyCardsAsync(long userId)
    {
        var now = DateTime.UtcNow;
        var cards = await db.UserPromoCodeCards
            .Include(c => c.PromoCode)
            .Where(c => c.UserId == userId
                && c.PromoCode.IsActive
                && !c.PromoCode.IsDeleted
                && (c.PromoCode.EndsAt == null || c.PromoCode.EndsAt >= now)
                && (c.PromoCode.MaxUsages == null || c.PromoCode.UsagesCount < c.PromoCode.MaxUsages))
            .ToListAsync();

        // Exclude already-used one-time-per-user codes
        var oneTimeCodeIds = cards
            .Where(c => c.PromoCode.IsOneTimePerUser)
            .Select(c => c.PromoCodeId)
            .ToList();

        HashSet<long> usedOneTimeIds = [];
        if (oneTimeCodeIds.Count > 0)
        {
            usedOneTimeIds = (await db.PromoCodeUsages
                .Where(u => u.UserId == userId && oneTimeCodeIds.Contains(u.PromoCodeId))
                .Select(u => u.PromoCodeId)
                .ToListAsync()).ToHashSet();
        }

        return cards
            .Where(c => !c.PromoCode.IsOneTimePerUser || !usedOneTimeIds.Contains(c.PromoCodeId))
            .Select(c => MapCard(c.PromoCode))
            .ToList();
    }

    public async Task<PromoCodeCardResponse> ActivatePromoCodeAsync(string code, long userId)
    {
        var normalized = code.Trim().ToUpper();
        var now = DateTime.UtcNow;

        var promoCode = await db.PromoCodes
            .FirstOrDefaultAsync(p => p.Code == normalized
                && p.IsActive
                && (p.StartsAt == null || p.StartsAt <= now)
                && (p.EndsAt == null || p.EndsAt >= now)
                && (p.MaxUsages == null || p.UsagesCount < p.MaxUsages))
            ?? throw new InvalidOperationException("Промокод не знайдено або недійсний");

        var alreadyActivated = await db.UserPromoCodeCards
            .AnyAsync(c => c.UserId == userId && c.PromoCodeId == promoCode.Id);

        if (!alreadyActivated)
        {
            db.UserPromoCodeCards.Add(new UserPromoCodeCard
            {
                UserId = userId,
                PromoCodeId = promoCode.Id,
                ActivatedAt = now
            });
            await db.SaveChangesAsync();
        }

        return MapCard(promoCode);
    }

    // ─── Checkout ──────────────────────────────────────────────────────────────

    public async Task<CalculateDiscountResponse> CalculateDiscountAsync(
        decimal orderTotal, long? userPromoCardId, long? userId)
    {
        var now = DateTime.UtcNow;

        // Find best active global promotion
        var promotions = await db.Promotions
            .Where(p => p.IsActive
                && p.Scope == PromotionScope.Global
                && (p.StartsAt == null || p.StartsAt <= now)
                && (p.EndsAt == null || p.EndsAt >= now)
                && (p.MinOrderAmount == null || p.MinOrderAmount <= orderTotal))
            .ToListAsync();

        if (userId.HasValue)
        {
            var usedIds = (await db.PromotionUsages
                .Where(u => u.UserId == userId)
                .Select(u => u.PromotionId)
                .ToListAsync()).ToHashSet();

            promotions = promotions
                .Where(p => !p.IsOneTimePerUser || !usedIds.Contains(p.Id))
                .ToList();
        }
        else
        {
            promotions = promotions.Where(p => !p.IsOneTimePerUser).ToList();
        }

        Promotion? bestPromotion = null;
        decimal promotionDiscount = 0;
        foreach (var p in promotions)
        {
            var discount = p.DiscountType == DiscountType.Percentage
                ? Math.Round(orderTotal * p.DiscountValue / 100, 2)
                : p.DiscountValue;

            if (discount > promotionDiscount)
            {
                promotionDiscount = discount;
                bestPromotion = p;
            }
        }

        // Apply promo code card
        decimal promoCodeDiscount = 0;
        PromoCode? appliedCode = null;

        if (userPromoCardId.HasValue && userId.HasValue)
        {
            var card = await db.UserPromoCodeCards
                .Include(c => c.PromoCode)
                .FirstOrDefaultAsync(c => c.PromoCodeId == userPromoCardId && c.UserId == userId);

            if (card != null)
            {
                var pc = card.PromoCode;
                var baseAmount = orderTotal - promotionDiscount;

                bool valid = pc.IsActive && !pc.IsDeleted
                    && (pc.StartsAt == null || pc.StartsAt <= now)
                    && (pc.EndsAt == null || pc.EndsAt >= now)
                    && (pc.MaxUsages == null || pc.UsagesCount < pc.MaxUsages)
                    && (pc.MinOrderAmount == null || pc.MinOrderAmount <= orderTotal);

                if (valid && pc.IsOneTimePerUser)
                    valid = !await db.PromoCodeUsages.AnyAsync(u => u.PromoCodeId == pc.Id && u.UserId == userId);

                if (valid)
                {
                    promoCodeDiscount = pc.DiscountType == DiscountType.Percentage
                        ? Math.Round(baseAmount * pc.DiscountValue / 100, 2)
                        : pc.DiscountValue;
                    promoCodeDiscount = Math.Min(promoCodeDiscount, baseAmount);
                    appliedCode = pc;
                }
            }
        }

        var totalDiscount = Math.Min(promotionDiscount + promoCodeDiscount, orderTotal);

        return new CalculateDiscountResponse
        {
            OriginalTotal = orderTotal,
            PromotionDiscount = promotionDiscount,
            PromoCodeDiscount = promoCodeDiscount,
            TotalDiscount = totalDiscount,
            FinalTotal = orderTotal - totalDiscount,
            AppliedPromotion = bestPromotion != null ? MapPublicPromotion(bestPromotion) : null,
            AppliedPromoCode = appliedCode != null ? MapCard(appliedCode) : null
        };
    }

    public async Task RecordUsagesAsync(
        long orderId, long? userId,
        long? promotionId, decimal promotionDiscount,
        long? promoCodeId, decimal promoCodeDiscount)
    {
        if (promotionId.HasValue && promotionDiscount > 0)
        {
            db.PromotionUsages.Add(new PromotionUsage
            {
                PromotionId = promotionId.Value,
                UserId = userId,
                OrderId = orderId,
                DiscountAmount = promotionDiscount,
                UsedAt = DateTime.UtcNow
            });
        }

        if (promoCodeId.HasValue && promoCodeDiscount > 0)
        {
            db.PromoCodeUsages.Add(new PromoCodeUsage
            {
                PromoCodeId = promoCodeId.Value,
                UserId = userId,
                OrderId = orderId,
                DiscountAmount = promoCodeDiscount,
                UsedAt = DateTime.UtcNow
            });

            var code = await db.PromoCodes.FindAsync(promoCodeId.Value);
            if (code != null) code.UsagesCount++;
        }

        await db.SaveChangesAsync();
    }

    // ─── Mappers ───────────────────────────────────────────────────────────────

    private static string ComputeStatus(bool isActive, DateTime? startsAt, DateTime? endsAt)
    {
        var now = DateTime.UtcNow;
        if (!isActive) return "inactive";
        if (endsAt.HasValue && endsAt < now) return "expired";
        if (startsAt.HasValue && startsAt > now) return "upcoming";
        return "active";
    }

    private static AdminPromotionResponse MapAdminPromotion(Promotion p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        DiscountType = p.DiscountType.ToString(),
        DiscountValue = p.DiscountValue,
        Scope = p.Scope.ToString(),
        CategoryId = p.CategoryId,
        CategoryName = p.Category?.Name,
        SubcategoryId = p.SubcategoryId,
        SubcategoryName = p.Subcategory?.Name,
        ProductId = p.ProductId,
        ProductName = p.Product?.Name,
        MinOrderAmount = p.MinOrderAmount,
        StartsAt = p.StartsAt,
        EndsAt = p.EndsAt,
        IsActive = p.IsActive,
        IsOneTimePerUser = p.IsOneTimePerUser,
        CreatedAt = p.CreatedAt,
        Status = ComputeStatus(p.IsActive, p.StartsAt, p.EndsAt)
    };

    private static AdminPromoCodeResponse MapAdminPromoCode(PromoCode p) => new()
    {
        Id = p.Id,
        Code = p.Code,
        DisplayName = p.DisplayName,
        CardColor = p.CardColor,
        Description = p.Description,
        DiscountType = p.DiscountType.ToString(),
        DiscountValue = p.DiscountValue,
        MinOrderAmount = p.MinOrderAmount,
        MaxUsages = p.MaxUsages,
        UsagesCount = p.UsagesCount,
        IsOneTimePerUser = p.IsOneTimePerUser,
        StartsAt = p.StartsAt,
        EndsAt = p.EndsAt,
        IsActive = p.IsActive,
        CreatedAt = p.CreatedAt,
        Status = ComputeStatus(p.IsActive, p.StartsAt, p.EndsAt)
    };

    private static PublicPromotionResponse MapPublicPromotion(Promotion p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        DiscountType = p.DiscountType.ToString(),
        DiscountValue = p.DiscountValue,
        Scope = p.Scope.ToString(),
        MinOrderAmount = p.MinOrderAmount,
        StartsAt = p.StartsAt,
        EndsAt = p.EndsAt,
        Status = ComputeStatus(p.IsActive, p.StartsAt, p.EndsAt)
    };

    private static PromoCodeCardResponse MapCard(PromoCode p) => new()
    {
        Id = p.Id,
        DisplayName = p.DisplayName,
        CardColor = p.CardColor,
        Description = p.Description,
        DiscountType = p.DiscountType.ToString(),
        DiscountValue = p.DiscountValue,
        MinOrderAmount = p.MinOrderAmount,
        EndsAt = p.EndsAt
    };

    private static DiscountType ParseDiscountType(string value) =>
        value == "FixedAmount" ? DiscountType.FixedAmount : DiscountType.Percentage;

    private static PromotionScope ParseScope(string value) => value switch
    {
        "Category" => PromotionScope.Category,
        "Subcategory" => PromotionScope.Subcategory,
        "Product" => PromotionScope.Product,
        _ => PromotionScope.Global
    };
}
