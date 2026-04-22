namespace VypusknykPlus.Application.DTOs.Admin;

public class RoleResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string[] Pages { get; set; } = [];
    public bool IsSuperAdmin { get; set; }
    public DateTime CreatedAt { get; set; }
}
