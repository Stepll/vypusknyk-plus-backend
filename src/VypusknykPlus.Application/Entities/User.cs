namespace VypusknykPlus.Application.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<SavedDesign> SavedDesigns { get; set; } = [];
    public ICollection<CartItem> CartItems { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
