namespace VypusknykPlus.Application.DTOs.Admin;

public class CreateRoleRequest
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string[] Pages { get; set; } = [];
}
