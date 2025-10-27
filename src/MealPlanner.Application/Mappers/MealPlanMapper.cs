using MealPlanner.Application.DTOs.MealPlans;
using MealPlanner.Domain.Entities;

namespace MealPlanner.Application.Mappers;

public static class MealPlanMapper
{
    public static MealPlanDto ToDto(MealPlan mealPlan)
    {
        return new MealPlanDto
        {
            Id = mealPlan.Id,
            HouseholdId = mealPlan.HouseholdId,
            WeekStartDate = mealPlan.WeekStartDate,
            CreatedAt = mealPlan.CreatedAt,
            PlannedMeals = mealPlan.PlannedMeals.Select(ToPlannedMealDto).ToList()
        };
    }

    public static PlannedMealDto ToPlannedMealDto(PlannedMeal plannedMeal)
    {
        return new PlannedMealDto
        {
            Id = plannedMeal.Id,
            MealPlanId = plannedMeal.MealPlanId,
            RecipeId = plannedMeal.RecipeId,
            RecipeName = plannedMeal.Recipe?.Name ?? string.Empty,
            DayOfWeek = plannedMeal.DayOfWeek,
            MealType = plannedMeal.MealType,
            Servings = plannedMeal.Servings,
            Notes = plannedMeal.Notes
        };
    }

    public static MealPlan ToEntity(SaveMealPlanDto dto, int householdId)
    {
        return new MealPlan
        {
            HouseholdId = householdId,
            WeekStartDate = dto.WeekStartDate,
            CreatedAt = DateTime.UtcNow,
            PlannedMeals = dto.PlannedMeals.Select(ToPlannedMealEntity).ToList()
        };
    }

    public static PlannedMeal ToPlannedMealEntity(SavePlannedMealDto dto)
    {
        return new PlannedMeal
        {
            RecipeId = dto.RecipeId,
            DayOfWeek = dto.DayOfWeek,
            MealType = dto.MealType,
            Servings = dto.Servings,
            Notes = dto.Notes
        };
    }
}
