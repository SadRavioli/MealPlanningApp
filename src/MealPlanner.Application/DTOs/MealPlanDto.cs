namespace MealPlanner.Application.DTOs;

public class MealPlanDto
{
    public int Id { get; set; }
    public int HouseholdId { get; set; }
    public DateTime WeekStartDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<PlannedMealDto> PlannedMeals { get; set; } = new();
}
