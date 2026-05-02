namespace VypusknykPlus.Application.Entities;

public class User : BaseEntity
{
    public bool IsGuest { get; set; }
    public string? Email { get; set; }
    public bool IsEmailVerified { get; set; }
    public string FullName { get; set; } = string.Empty;
    public bool IsNameVerified { get; set; }
    public string? Phone { get; set; }
    public bool IsPhoneVerified { get; set; }
    public string? PasswordHash { get; set; }
    public string? GoogleId { get; set; }

    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<SavedDesign> SavedDesigns { get; set; } = [];
    public ICollection<CartItem> CartItems { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
