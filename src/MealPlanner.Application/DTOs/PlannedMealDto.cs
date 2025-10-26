using MealPlanner.Domain.Enums;

namespace MealPlanner.Application.DTOs;

public class PlannedMealDto
{
    public int Id { get; set; }
    public int MealPlanId { get; set; }
    public int RecipeId { get; set; }
    public string RecipeName { get; set; } = string.Empty; // Include recipe name for display
    public DayOfWeek DayOfWeek { get; set; }
    public MealType MealType { get; set; }
    public int ServingsPlanned { get; set; }
    public string? Notes { get; set; }
}
