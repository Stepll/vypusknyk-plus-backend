using VypusknykPlus.Application.DTOs.Promotions;

namespace VypusknykPlus.Application.Services;

public interface IPromotionService
{
    // Admin — Promotions
    Task<List<AdminPromotionResponse>> GetAdminPromotionsAsync();
    Task<AdminPromotionResponse> CreatePromotionAsync(SavePromotionRequest request);
    Task<AdminPromotionResponse> UpdatePromotionAsync(long id, SavePromotionRequest request);
    Task DeletePromotionAsync(long id);

    // Admin — Promo codes
    Task<List<AdminPromoCodeResponse>> GetAdminPromoCodesAsync();
    Task<AdminPromoCodeResponse> CreatePromoCodeAsync(SavePromoCodeRequest request);
    Task<AdminPromoCodeResponse> UpdatePromoCodeAsync(long id, SavePromoCodeRequest request);
    Task DeletePromoCodeAsync(long id);

    // Public
    Task<List<PublicPromotionResponse>> GetPublicPromotionsAsync();
    Task<List<PromoCodeCardResponse>> GetMyCardsAsync(long userId);
    Task<PromoCodeCardResponse> ActivatePromoCodeAsync(string code, long userId);

    // Checkout
    Task<CalculateDiscountResponse> CalculateDiscountAsync(decimal orderTotal, long? userPromoCardId, long? userId);
    Task RecordUsagesAsync(long orderId, long? userId, long? promotionId, decimal promotionDiscount, long? promoCodeId, decimal promoCodeDiscount);
}
