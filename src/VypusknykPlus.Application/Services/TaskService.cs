using Microsoft.EntityFrameworkCore;
using VypusknykPlus.Application.Data;
using VypusknykPlus.Application.DTOs.Tasks;
using VypusknykPlus.Application.Entities;

namespace VypusknykPlus.Application.Services;

public class TaskService(AppDbContext db) : ITaskService
{
    // ─── Admin ────────────────────────────────────────────────────────────────

    public async Task<List<AdminTaskResponse>> GetAdminTasksAsync()
    {
        var tasks = await db.UserTasks
            .IgnoreQueryFilters()
            .Where(t => !t.IsDeleted)
            .Include(t => t.TargetCategory)
            .Include(t => t.RewardPromoCode)
            .Include(t => t.Progresses)
            .OrderBy(t => t.SortOrder).ThenByDescending(t => t.CreatedAt)
            .ToListAsync();

        return tasks.Select(MapAdmin).ToList();
    }

    public async Task<AdminTaskResponse> CreateTaskAsync(SaveTaskRequest request)
    {
        var entity = new UserTask
        {
            Name = request.Name,
            Description = request.Description,
            TaskType = ParseType(request.TaskType),
            TargetValue = request.TargetValue,
            TargetCategoryId = request.TargetCategoryId,
            RewardPromoCodeId = request.RewardPromoCodeId,
            IsVisibleToGuests = request.IsVisibleToGuests,
            EndsAt = request.EndsAt,
            IsActive = request.IsActive,
            SortOrder = request.SortOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        db.UserTasks.Add(entity);
        await db.SaveChangesAsync();
        return await ReloadAdmin(entity.Id);
    }

    public async Task<AdminTaskResponse> UpdateTaskAsync(long id, SaveTaskRequest request)
    {
        var entity = await db.UserTasks.IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted)
            ?? throw new KeyNotFoundException($"Task {id} not found");

        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.TaskType = ParseType(request.TaskType);
        entity.TargetValue = request.TargetValue;
        entity.TargetCategoryId = request.TargetCategoryId;
        entity.RewardPromoCodeId = request.RewardPromoCodeId;
        entity.IsVisibleToGuests = request.IsVisibleToGuests;
        entity.EndsAt = request.EndsAt;
        entity.IsActive = request.IsActive;
        entity.SortOrder = request.SortOrder;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return await ReloadAdmin(entity.Id);
    }

    public async Task DeleteTaskAsync(long id)
    {
        var entity = await db.UserTasks.IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted)
            ?? throw new KeyNotFoundException($"Task {id} not found");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    // ─── Public ───────────────────────────────────────────────────────────────

    public async Task<List<PublicTaskResponse>> GetPublicTasksAsync(long? userId)
    {
        var now = DateTime.UtcNow;
        var tasks = await db.UserTasks
            .Include(t => t.TargetCategory)
            .Include(t => t.RewardPromoCode)
            .Where(t => t.IsActive && (t.EndsAt == null || t.EndsAt >= now))
            .OrderBy(t => t.SortOrder)
            .ToListAsync();

        if (userId == null)
            tasks = tasks.Where(t => t.IsVisibleToGuests).ToList();

        Dictionary<long, UserTaskProgress> progresses = [];
        if (userId.HasValue)
        {
            var ids = tasks.Select(t => t.Id).ToList();
            progresses = await db.UserTaskProgresses
                .Where(p => p.UserId == userId && ids.Contains(p.TaskId))
                .ToDictionaryAsync(p => p.TaskId);
        }

        return tasks
            .Where(t =>
            {
                progresses.TryGetValue(t.Id, out var prog);
                return prog?.CompletedAt == null;
            })
            .Select(t =>
            {
                progresses.TryGetValue(t.Id, out var prog);
                return MapPublic(t, prog);
            }).ToList();
    }

    // ─── Task checking ────────────────────────────────────────────────────────

    public async Task CheckAndAwardAsync(long userId, TaskTrigger trigger)
    {
        var now = DateTime.UtcNow;
        var tasks = await db.UserTasks
            .Include(t => t.RewardPromoCode)
            .Where(t => t.IsActive && (t.EndsAt == null || t.EndsAt >= now))
            .ToListAsync();

        var completedIds = await db.UserTaskProgresses
            .Where(p => p.UserId == userId && p.CompletedAt != null)
            .Select(p => p.TaskId)
            .ToListAsync();

        var pending = tasks.Where(t => !completedIds.Contains(t.Id)).ToList();
        if (!pending.Any()) return;

        // Pre-load stats we might need
        int? ordersCount = null;
        decimal? totalSpent = null;

        foreach (var task in pending)
        {
            bool eligible = task.TaskType switch
            {
                TaskType.Registration => trigger.IsRegistration,
                TaskType.FirstOrder => trigger.IsOrderPlaced,
                TaskType.ProfileComplete => trigger.IsProfileUpdated,
                TaskType.AccountActivation => trigger.IsEmailVerified,
                TaskType.OrderAmount => trigger.IsOrderPlaced && trigger.OrderAmount >= task.TargetValue,
                TaskType.OrdersCount => trigger.IsOrderPlaced,
                TaskType.TotalSpent => trigger.IsOrderPlaced,
                TaskType.CategoryOrders => trigger.IsOrderPlaced && trigger.OrderCategoryId == task.TargetCategoryId,
                _ => false
            };
            if (!eligible) continue;

            decimal currentProgress = task.TaskType switch
            {
                TaskType.Registration => 1,
                TaskType.FirstOrder => 1,
                TaskType.ProfileComplete => await CheckProfileCompleteAsync(userId) ? 1 : 0,
                TaskType.AccountActivation => 1,
                TaskType.OrderAmount => trigger.OrderAmount,
                TaskType.OrdersCount => ordersCount ??= await GetOrdersCountAsync(userId),
                TaskType.TotalSpent => totalSpent ??= await GetTotalSpentAsync(userId),
                TaskType.CategoryOrders => await GetCategoryOrdersCountAsync(userId, task.TargetCategoryId),
                _ => 0
            };

            bool completed = currentProgress >= task.TargetValue;

            var existingProgress = await db.UserTaskProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.TaskId == task.Id);

            if (existingProgress == null)
            {
                existingProgress = new UserTaskProgress
                {
                    TaskId = task.Id,
                    UserId = userId,
                    Progress = currentProgress,
                    CompletedAt = completed ? now : null,
                };
                db.UserTaskProgresses.Add(existingProgress);
            }
            else
            {
                existingProgress.Progress = currentProgress;
                if (completed && existingProgress.CompletedAt == null)
                    existingProgress.CompletedAt = now;
            }

            if (completed && existingProgress.AwardedCardId == null)
            {
                var card = new UserPromoCodeCard
                {
                    UserId = userId,
                    PromoCodeId = task.RewardPromoCodeId,
                    ActivatedAt = now,
                };
                db.UserPromoCodeCards.Add(card);
                await db.SaveChangesAsync();
                existingProgress.AwardedCardId = card.Id;
            }
        }

        await db.SaveChangesAsync();
    }

    // ─── Private helpers ──────────────────────────────────────────────────────

    private async Task<bool> CheckProfileCompleteAsync(long userId)
    {
        var user = await db.Users.FindAsync(userId);
        return user != null && !string.IsNullOrWhiteSpace(user.FullName) && !string.IsNullOrWhiteSpace(user.Phone);
    }

    private async Task<int> GetOrdersCountAsync(long userId) =>
        await db.Orders.CountAsync(o => o.UserId == userId);

    private async Task<decimal> GetTotalSpentAsync(long userId) =>
        await db.Orders.Where(o => o.UserId == userId).SumAsync(o => (decimal?)o.Total) ?? 0;

    private async Task<decimal> GetCategoryOrdersCountAsync(long userId, long? categoryId)
    {
        if (categoryId == null) return 0;
        var category = await db.ProductCategories.FindAsync(categoryId);
        if (category == null) return 0;
        return await db.Orders
            .Where(o => o.UserId == userId)
            .SelectMany(o => o.Items)
            .Where(i => i.ProductCategory == category.Name)
            .Select(i => i.OrderId)
            .Distinct()
            .CountAsync();
    }

    // ─── Mapping ──────────────────────────────────────────────────────────────

    private static AdminTaskResponse MapAdmin(UserTask t) => new()
    {
        Id = t.Id,
        Name = t.Name,
        Description = t.Description,
        TaskType = t.TaskType.ToString(),
        TargetValue = t.TargetValue,
        TargetCategoryId = t.TargetCategoryId,
        TargetCategoryName = t.TargetCategory?.Name,
        RewardPromoCodeId = t.RewardPromoCodeId,
        RewardPromoCodeDisplayName = t.RewardPromoCode.DisplayName,
        RewardPromoCodeCardColor = t.RewardPromoCode.CardColor,
        RewardDiscountType = t.RewardPromoCode.DiscountType.ToString(),
        RewardDiscountValue = t.RewardPromoCode.DiscountValue,
        IsVisibleToGuests = t.IsVisibleToGuests,
        EndsAt = t.EndsAt,
        IsActive = t.IsActive,
        SortOrder = t.SortOrder,
        CompletionsCount = t.Progresses.Count(p => p.CompletedAt != null),
        CreatedAt = t.CreatedAt,
    };

    private static PublicTaskResponse MapPublic(UserTask t, UserTaskProgress? prog) => new()
    {
        Id = t.Id,
        Name = t.Name,
        Description = t.Description,
        TaskType = t.TaskType.ToString(),
        TargetValue = t.TargetValue,
        TargetCategoryId = t.TargetCategoryId,
        TargetCategoryName = t.TargetCategory?.Name,
        IsVisibleToGuests = t.IsVisibleToGuests,
        EndsAt = t.EndsAt,
        RewardDisplayName = t.RewardPromoCode.DisplayName,
        RewardCardColor = t.RewardPromoCode.CardColor,
        RewardDiscountType = t.RewardPromoCode.DiscountType.ToString(),
        RewardDiscountValue = t.RewardPromoCode.DiscountValue,
        UserProgress = prog?.Progress,
        IsCompleted = prog?.CompletedAt != null,
    };

    private async Task<AdminTaskResponse> ReloadAdmin(long id)
    {
        var t = await db.UserTasks
            .Include(t => t.TargetCategory)
            .Include(t => t.RewardPromoCode)
            .Include(t => t.Progresses)
            .FirstAsync(t => t.Id == id);
        return MapAdmin(t);
    }

    private static TaskType ParseType(string value) => value switch
    {
        "Registration" => TaskType.Registration,
        "FirstOrder" => TaskType.FirstOrder,
        "ProfileComplete" => TaskType.ProfileComplete,
        "OrdersCount" => TaskType.OrdersCount,
        "TotalSpent" => TaskType.TotalSpent,
        "OrderAmount" => TaskType.OrderAmount,
        "CategoryOrders" => TaskType.CategoryOrders,
        "AccountActivation" => TaskType.AccountActivation,
        _ => TaskType.Registration,
    };
}
