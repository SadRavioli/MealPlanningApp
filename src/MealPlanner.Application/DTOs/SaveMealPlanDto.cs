using MealPlanner.Domain.Enums;

namespace MealPlanner.Application.DTOs;

public class SaveMealPlanDto
{
    public DateTime WeekStartDate { get; set; }
    public List<SavePlannedMealDto> PlannedMeals { get; set; } = new();
}

public class SavePlannedMealDto
{
    public int RecipeId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public MealType MealType { get; set; }
    public int Servings { get; set; }
    public string? Notes { get; set; }
}
