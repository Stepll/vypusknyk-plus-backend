namespace VypusknykPlus.Application.DTOs.Admin;

public class AdminUserResponse
{
    public long Id { get; set; }
    public bool IsGuest { get; set; }
    public string? Email { get; set; }
    public bool IsEmailVerified { get; set; }
    public string FullName { get; set; } = string.Empty;
    public bool IsNameVerified { get; set; }
    public string? Phone { get; set; }
    public bool IsPhoneVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public int OrdersCount { get; set; }
}
