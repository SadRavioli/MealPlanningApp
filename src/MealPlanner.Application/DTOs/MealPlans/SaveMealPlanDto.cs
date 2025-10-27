using MealPlanner.Domain.Enums;

namespace MealPlanner.Application.DTOs.MealPlans;

public class SaveMealPlanDto
{
    public DateTime WeekStartDate { get; set; }
    public IEnumerable<SavePlannedMealDto> PlannedMeals { get; set; } = new List<SavePlannedMealDto>();
}
