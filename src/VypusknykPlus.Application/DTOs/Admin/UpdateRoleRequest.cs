namespace VypusknykPlus.Application.DTOs.Admin;

public class UpdateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string[] Pages { get; set; } = [];
}
