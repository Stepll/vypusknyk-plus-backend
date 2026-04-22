namespace VypusknykPlus.Application.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string[] Pages { get; set; } = [];
    public bool IsSuperAdmin { get; set; }
    public ICollection<Admin> Admins { get; set; } = [];
}
