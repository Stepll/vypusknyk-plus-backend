namespace VypusknykPlus.Application.DTOs.Admin;

public class AdminUserOrderSummary
{
    public long Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public int ItemsCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AdminUserSavedDesign
{
    public long Id { get; set; }
    public string DesignName { get; set; } = string.Empty;
    public DateTime SavedAt { get; set; }
}

public class AdminUserDetailResponse
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
    public List<AdminUserOrderSummary> Orders { get; set; } = [];
    public List<AdminUserSavedDesign> SavedDesigns { get; set; } = [];
}

public class PatchUserInfoRequest
{
    public string? FullName { get; set; }
    public string? Phone { get; set; }
}

public class PatchUserVerificationRequest
{
    public bool? IsEmailVerified { get; set; }
    public bool? IsNameVerified { get; set; }
    public bool? IsPhoneVerified { get; set; }
}
