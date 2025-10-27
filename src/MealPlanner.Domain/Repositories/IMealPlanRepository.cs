using MealPlanner.Domain.Entities;

namespace MealPlanner.Domain.Repositories;

public interface IMealPlanRepository : IRepository<MealPlan>
{
    Task<IReadOnlyList<MealPlan>> GetByHouseholdIdAsync(int householdId, CancellationToken cancellationToken = default);
    Task<MealPlan?> GetByWeekStartDateAsync(int householdId, DateTime weekStartDate, CancellationToken cancellationToken = default);
    Task<MealPlan?> GetByIdWithMealsAsync(int id, CancellationToken cancellationToken = default);
}
