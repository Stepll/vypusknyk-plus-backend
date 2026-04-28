namespace VypusknykPlus.Application.Entities;

public class User : BaseEntity
{
    public bool IsGuest { get; set; }
    public string? Email { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? PasswordHash { get; set; }

    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<SavedDesign> SavedDesigns { get; set; } = [];
    public ICollection<CartItem> CartItems { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
