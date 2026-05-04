using VypusknykPlus.Application.DTOs.Tasks;

namespace VypusknykPlus.Application.Services;

public interface ITaskService
{
    Task<List<AdminTaskResponse>> GetAdminTasksAsync();
    Task<AdminTaskResponse> CreateTaskAsync(SaveTaskRequest request);
    Task<AdminTaskResponse> UpdateTaskAsync(long id, SaveTaskRequest request);
    Task DeleteTaskAsync(long id);

    Task<List<PublicTaskResponse>> GetPublicTasksAsync(long? userId);
    Task CheckAndAwardAsync(long userId, TaskTrigger trigger);
}

public class TaskTrigger
{
    public bool IsRegistration { get; set; }
    public bool IsProfileUpdated { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsOrderPlaced { get; set; }
    public decimal OrderAmount { get; set; }
    public long? OrderCategoryId { get; set; }
}
