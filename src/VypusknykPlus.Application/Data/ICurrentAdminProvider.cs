namespace VypusknykPlus.Application.Data;

public interface ICurrentAdminProvider
{
    (long? AdminId, string AdminName) GetCurrent();
}
