namespace VypusknykPlus.Application.DTOs.Admin;

public class AdminUserResponse
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public int OrdersCount { get; set; }
}
