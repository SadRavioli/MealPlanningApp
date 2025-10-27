using MealPlanner.Application.DTOs.MealPlans;

namespace MealPlanner.Application.Services;

public interface IMealPlanService
{
    Task<MealPlanDto?> GetMealPlanByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MealPlanDto>> GetMealPlansByHouseholdAsync(int householdId, CancellationToken cancellationToken = default);
    Task<MealPlanDto> CreateMealPlanAsync(int householdId, SaveMealPlanDto dto, CancellationToken cancellationToken = default);
    Task UpdateMealPlanAsync(int id, SaveMealPlanDto dto, CancellationToken cancellationToken = default);
    Task DeleteMealPlanAsync(int id, CancellationToken cancellationToken = default);
}
