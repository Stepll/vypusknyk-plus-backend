namespace VypusknykPlus.Application.ValueObjects;

public class ClassGroup
{
    public string ClassName { get; set; } = string.Empty;
    public string Names { get; set; } = string.Empty;
}

public class NamesData
{
    public string School { get; set; } = string.Empty;
    public List<ClassGroup> Groups { get; set; } = [];
}
