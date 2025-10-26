using MealPlanner.Application.DTOs;
using MealPlanner.Application.Mappers;
using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Enums;
using MealPlanner.Domain.Repositories;

namespace MealPlanner.Application.Services;

public class MealPlanService : IMealPlanService
{
    private readonly IMealPlanRepository _mealPlanRepository;

    public MealPlanService(IMealPlanRepository mealPlanRepository)
    {
        _mealPlanRepository = mealPlanRepository;
    }

    public async Task<MealPlanDto?> GetMealPlanByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var mealPlan = await _mealPlanRepository.GetByIdAsync(id, cancellationToken);
        return mealPlan == null ? null : MealPlanMapper.ToDto(mealPlan);
    }

    public async Task<IEnumerable<MealPlanDto>> GetMealPlansByHouseholdAsync(int householdId, CancellationToken cancellationToken = default)
    {
        var mealPlans = await _mealPlanRepository.GetByHouseholdIdAsync(householdId, cancellationToken);
        return mealPlans.Select(MealPlanMapper.ToDto);
    }

    public async Task<MealPlanDto> CreateMealPlanAsync(int householdId, SaveMealPlanDto dto, CancellationToken cancellationToken = default)
    {
        var mealPlan = MealPlanMapper.ToEntity(dto, householdId);
        var created = await _mealPlanRepository.AddAsync(mealPlan, cancellationToken);
        return MealPlanMapper.ToDto(created);
    }

    public async Task UpdateMealPlanAsync(int id, SaveMealPlanDto dto, CancellationToken cancellationToken = default)
    {
        var mealPlan = await _mealPlanRepository.GetByIdAsync(id, cancellationToken);
        if (mealPlan == null)
            throw new KeyNotFoundException($"MealPlan with ID {id} not found");

        // Update meal plan properties
        mealPlan.WeekStartDate = dto.WeekStartDate;

        // Update planned meals
        mealPlan.PlannedMeals = [.. dto.PlannedMeals.Select(pm => 
        {
            var plannedMeal = MealPlanMapper.ToPlannedMealEntity(pm);
            plannedMeal.MealPlanId = mealPlan.Id;
            return plannedMeal;
        })];

        await _mealPlanRepository.UpdateAsync(mealPlan, cancellationToken);
    }

    public async Task DeleteMealPlanAsync(int id, CancellationToken cancellationToken = default)
    {
        var mealPlan = await _mealPlanRepository.GetByIdAsync(id, cancellationToken);
        if (mealPlan == null)
            throw new KeyNotFoundException($"MealPlan with ID {id} not found");

        await _mealPlanRepository.DeleteAsync(mealPlan, cancellationToken);
    }
}
