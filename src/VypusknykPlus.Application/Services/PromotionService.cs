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
            .Include(p => p.TargetCategories).ThenInclude(t => t.Category)
            .Include(p => p.TargetCategories).ThenInclude(t => t.Subcategory)
            .Include(p => p.VolumeTiers)
            .Include(p => p.BundleItems).ThenInclude(b => b.Subcategory)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return items.Select(MapAdminPromotion).ToList();
    }

    public async Task<AdminPromotionResponse> GetAdminPromotionAsync(long id) =>
        await ReloadPromotion(id);

    public async Task<AdminPromotionResponse> CreatePromotionAsync(SavePromotionRequest request)
    {
        var entity = new Promotion
        {
            Name = request.Name,
            Description = request.Description,
            DiscountType = ParseDiscountType(request.DiscountType),
            DiscountValue = request.DiscountValue,
            Scope = ParseScope(request.Scope),
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

        await SaveTargets(entity.Id, request);
        await db.SaveChangesAsync();

        return await ReloadPromotion(entity.Id);
    }

    public async Task<AdminPromotionResponse> UpdatePromotionAsync(long id, SavePromotionRequest request)
    {
        var entity = await db.Promotions
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted)
            ?? throw new KeyNotFoundException($"Promotion {id} not found");

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.DiscountType = ParseDiscountType(request.DiscountType);
        entity.DiscountValue = request.DiscountValue;
        entity.Scope = ParseScope(request.Scope);
        entity.MinOrderAmount = request.MinOrderAmount;
        entity.StartsAt = request.StartsAt;
        entity.EndsAt = request.EndsAt;
        entity.IsActive = request.IsActive;
        entity.IsOneTimePerUser = request.IsOneTimePerUser;
        entity.UpdatedAt = DateTime.UtcNow;

        // Replace targets, tiers, bundle items
        await db.PromotionTargetCategories.Where(t => t.PromotionId == id).ExecuteDeleteAsync();
        await db.PromotionVolumeTiers.Where(t => t.PromotionId == id).ExecuteDeleteAsync();
        await db.PromotionBundleItems.Where(b => b.PromotionId == id).ExecuteDeleteAsync();

        await db.SaveChangesAsync();
        await SaveTargets(id, request);
        await db.SaveChangesAsync();

        return await ReloadPromotion(id);
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
        var normalized = string.IsNullOrWhiteSpace(request.Code) ? null : request.Code.Trim().ToUpper();
        if (normalized != null && await db.PromoCodes.IgnoreQueryFilters().AnyAsync(p => p.Code == normalized && !p.IsDeleted))
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

        var normalized = string.IsNullOrWhiteSpace(request.Code) ? null : request.Code.Trim().ToUpper();
        if (normalized != null && await db.PromoCodes.IgnoreQueryFilters().AnyAsync(p => p.Code == normalized && p.Id != id && !p.IsDeleted))
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
        decimal orderTotal, long? userPromoCardId, long? userId, List<CartItemForDiscount>? items = null)
    {
        var now = DateTime.UtcNow;
        items ??= [];
        var productIds = items.Where(i => i.ProductId.HasValue).Select(i => i.ProductId!.Value).Distinct().ToList();

        // Resolve category/subcategory IDs for cart products
        Dictionary<long, (long CategoryId, long? SubcategoryId)> productCats = [];
        if (productIds.Count > 0)
        {
            var products = await db.Products
                .Where(p => productIds.Contains(p.Id))
                .Select(p => new { p.Id, p.CategoryId, p.SubcategoryId })
                .ToListAsync();
            productCats = products.ToDictionary(p => p.Id, p => (p.CategoryId, p.SubcategoryId));
        }

        HashSet<long> cartCategoryIds = [];
        HashSet<long> cartSubcategoryIds = [];
        foreach (var p in productCats.Values)
        {
            cartCategoryIds.Add(p.CategoryId);
            if (p.SubcategoryId.HasValue) cartSubcategoryIds.Add(p.SubcategoryId.Value);
        }

        // Load all active promotions with their targets
        var candidates = await db.Promotions
            .Include(p => p.TargetCategories)
            .Include(p => p.VolumeTiers)
            .Include(p => p.BundleItems)
            .Where(p => p.IsActive
                && (p.StartsAt == null || p.StartsAt <= now)
                && (p.EndsAt == null || p.EndsAt >= now))
            .ToListAsync();

        // Filter one-time-per-user
        if (userId.HasValue)
        {
            var usedIds = (await db.PromotionUsages
                .Where(u => u.UserId == userId)
                .Select(u => u.PromotionId)
                .ToListAsync()).ToHashSet();
            candidates = candidates.Where(p => !p.IsOneTimePerUser || !usedIds.Contains(p.Id)).ToList();
        }
        else
        {
            candidates = candidates.Where(p => !p.IsOneTimePerUser).ToList();
        }

        // Filter minOrderAmount
        candidates = candidates.Where(p => p.MinOrderAmount == null || p.MinOrderAmount <= orderTotal).ToList();

        // Calculate discount per promotion, pick best
        Promotion? bestPromotion = null;
        decimal promotionDiscount = 0;

        foreach (var p in candidates)
        {
            var discount = p.Scope switch
            {
                PromotionScope.Global => CalcDiscount(p.DiscountType, p.DiscountValue, orderTotal),
                PromotionScope.Category => CalcCategoryDiscount(p, items, productCats, cartCategoryIds, cartSubcategoryIds, orderTotal),
                PromotionScope.Volume => CalcVolumeDiscount(p, items, productCats, cartCategoryIds, cartSubcategoryIds),
                PromotionScope.Bundle => CalcBundleDiscount(p, items, productCats),
                _ => 0m
            };

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
                    promoCodeDiscount = CalcDiscount(pc.DiscountType, pc.DiscountValue, baseAmount);
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

    // ─── Discount calculators ──────────────────────────────────────────────────

    private static decimal CalcDiscount(DiscountType type, decimal value, decimal baseAmount) =>
        Math.Round(type == DiscountType.Percentage ? baseAmount * value / 100 : value, 2);

    private static decimal CalcCategoryDiscount(
        Promotion p, List<CartItemForDiscount> items,
        Dictionary<long, (long CategoryId, long? SubcategoryId)> productCats,
        HashSet<long> cartCategoryIds, HashSet<long> cartSubcategoryIds,
        decimal orderTotal)
    {
        var targetCatIds = p.TargetCategories.Where(t => t.CategoryId.HasValue).Select(t => t.CategoryId!.Value).ToHashSet();
        var targetSubIds = p.TargetCategories.Where(t => t.SubcategoryId.HasValue).Select(t => t.SubcategoryId!.Value).ToHashSet();

        bool applies = targetCatIds.Overlaps(cartCategoryIds) || targetSubIds.Overlaps(cartSubcategoryIds);
        if (!applies) return 0;

        // Discount applies to matching items only
        var matchingTotal = items
            .Where(i => i.ProductId.HasValue && productCats.TryGetValue(i.ProductId.Value, out var cat)
                && (targetCatIds.Contains(cat.CategoryId) || (cat.SubcategoryId.HasValue && targetSubIds.Contains(cat.SubcategoryId.Value))))
            .Sum(i => i.Qty * i.UnitPrice);

        return CalcDiscount(p.DiscountType, p.DiscountValue, matchingTotal);
    }

    private static decimal CalcVolumeDiscount(
        Promotion p, List<CartItemForDiscount> items,
        Dictionary<long, (long CategoryId, long? SubcategoryId)> productCats,
        HashSet<long> cartCategoryIds, HashSet<long> cartSubcategoryIds)
    {
        if (p.VolumeTiers.Count == 0) return 0;

        var targetCatIds = p.TargetCategories.Where(t => t.CategoryId.HasValue).Select(t => t.CategoryId!.Value).ToHashSet();
        var targetSubIds = p.TargetCategories.Where(t => t.SubcategoryId.HasValue).Select(t => t.SubcategoryId!.Value).ToHashSet();

        bool hasTargets = targetCatIds.Count > 0 || targetSubIds.Count > 0;

        var matchingItems = items.Where(i =>
        {
            if (!i.ProductId.HasValue) return !hasTargets;
            if (!productCats.TryGetValue(i.ProductId.Value, out var cat)) return false;
            if (!hasTargets) return true;
            return targetCatIds.Contains(cat.CategoryId) || (cat.SubcategoryId.HasValue && targetSubIds.Contains(cat.SubcategoryId.Value));
        }).ToList();

        var totalQty = matchingItems.Sum(i => i.Qty);
        var matchingTotal = matchingItems.Sum(i => i.Qty * i.UnitPrice);

        var bestTier = p.VolumeTiers
            .Where(t => t.MinQty <= totalQty)
            .OrderByDescending(t => t.MinQty)
            .FirstOrDefault();

        return bestTier == null ? 0 : CalcDiscount(bestTier.DiscountType, bestTier.DiscountValue, matchingTotal);
    }

    private static decimal CalcBundleDiscount(
        Promotion p, List<CartItemForDiscount> items,
        Dictionary<long, (long CategoryId, long? SubcategoryId)> productCats)
    {
        if (p.BundleItems.Count == 0) return 0;

        // Check each bundle requirement is satisfied
        foreach (var bundleItem in p.BundleItems)
        {
            var qtyInCart = items
                .Where(i => i.ProductId.HasValue
                    && productCats.TryGetValue(i.ProductId.Value, out var cat)
                    && cat.SubcategoryId == bundleItem.SubcategoryId)
                .Sum(i => i.Qty);

            if (qtyInCart < bundleItem.RequiredQty) return 0;
        }

        // Bundle satisfied — discount on sum of bundle subcategory items
        var bundleSubIds = p.BundleItems.Select(b => b.SubcategoryId).ToHashSet();
        var bundleTotal = items
            .Where(i => i.ProductId.HasValue
                && productCats.TryGetValue(i.ProductId.Value, out var cat)
                && cat.SubcategoryId.HasValue
                && bundleSubIds.Contains(cat.SubcategoryId.Value))
            .Sum(i => i.Qty * i.UnitPrice);

        return CalcDiscount(p.DiscountType, p.DiscountValue, bundleTotal);
    }

    // ─── Helpers ───────────────────────────────────────────────────────────────

    private async Task SaveTargets(long promotionId, SavePromotionRequest request)
    {
        foreach (var catId in request.TargetCategoryIds)
            db.PromotionTargetCategories.Add(new PromotionTargetCategory { PromotionId = promotionId, CategoryId = catId });

        foreach (var subId in request.TargetSubcategoryIds)
            db.PromotionTargetCategories.Add(new PromotionTargetCategory { PromotionId = promotionId, SubcategoryId = subId });

        foreach (var tier in request.VolumeTiers)
            db.PromotionVolumeTiers.Add(new PromotionVolumeTier
            {
                PromotionId = promotionId,
                MinQty = tier.MinQty,
                DiscountType = ParseDiscountType(tier.DiscountType),
                DiscountValue = tier.DiscountValue
            });

        foreach (var bi in request.BundleItems)
            db.PromotionBundleItems.Add(new PromotionBundleItem
            {
                PromotionId = promotionId,
                SubcategoryId = bi.SubcategoryId,
                RequiredQty = bi.RequiredQty
            });

        await Task.CompletedTask;
    }

    private async Task<AdminPromotionResponse> ReloadPromotion(long id)
    {
        var entity = await db.Promotions
            .Include(p => p.TargetCategories).ThenInclude(t => t.Category)
            .Include(p => p.TargetCategories).ThenInclude(t => t.Subcategory)
            .Include(p => p.VolumeTiers)
            .Include(p => p.BundleItems).ThenInclude(b => b.Subcategory)
            .FirstAsync(p => p.Id == id);
        return MapAdminPromotion(entity);
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
        Targets = p.TargetCategories.Select(t => new PromotionTargetDto
        {
            CategoryId = t.CategoryId,
            CategoryName = t.Category?.Name,
            SubcategoryId = t.SubcategoryId,
            SubcategoryName = t.Subcategory?.Name
        }).ToList(),
        VolumeTiers = p.VolumeTiers.OrderBy(t => t.MinQty).Select(t => new VolumeTierDto
        {
            Id = t.Id, MinQty = t.MinQty,
            DiscountType = t.DiscountType.ToString(), DiscountValue = t.DiscountValue
        }).ToList(),
        BundleItems = p.BundleItems.Select(b => new BundleItemDto
        {
            Id = b.Id, SubcategoryId = b.SubcategoryId,
            SubcategoryName = b.Subcategory?.Name ?? string.Empty, RequiredQty = b.RequiredQty
        }).ToList(),
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
        "Volume" => PromotionScope.Volume,
        "Bundle" => PromotionScope.Bundle,
        _ => PromotionScope.Global
    };
}
