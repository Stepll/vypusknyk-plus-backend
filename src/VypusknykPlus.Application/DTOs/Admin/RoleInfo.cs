namespace VypusknykPlus.Application.DTOs.Admin;

public class RoleInfo
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public bool IsSuperAdmin { get; set; }
    public string[] Pages { get; set; } = [];
}
