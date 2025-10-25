using MealPlanner.Domain.Enums;

namespace MealPlanner.Domain.Entities;

public class PlannedMeal
{
    public int Id { get; set; }
    public int MealPlanId { get; set; }
    public int RecipeId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public MealType MealType { get; set; }
    public int ServingsPlanned { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public MealPlan MealPlan { get; set; } = null!;
    public Recipe Recipe { get; set; } = null!;
}
