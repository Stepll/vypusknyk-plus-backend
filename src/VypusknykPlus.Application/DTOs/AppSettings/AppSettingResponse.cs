namespace VypusknykPlus.Application.DTOs.AppSettings;

public record AppSettingResponse(
    string Key,
    string Value,
    string Type,
    string Group,
    string Label,
    string? Description,
    DateTime UpdatedAt
);
