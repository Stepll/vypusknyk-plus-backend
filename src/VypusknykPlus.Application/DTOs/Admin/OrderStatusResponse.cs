using System.ComponentModel.DataAnnotations;

namespace VypusknykPlus.Application.DTOs.Admin;

public class OrderStatusResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsFinal { get; set; }
    public bool IsActive { get; set; }
}

public class SaveOrderStatusRequest
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(50)]
    public string Color { get; set; } = string.Empty;

    [Range(1, 100)]
    public int SortOrder { get; set; }

    public bool IsFinal { get; set; }
    public bool IsActive { get; set; } = true;
}
