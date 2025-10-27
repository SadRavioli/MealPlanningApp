namespace MealPlanner.Application.DTOs;

public class MealPlanDto
{
    public int Id { get; set; }
    public int HouseholdId { get; set; }
    public DateTime WeekStartDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<PlannedMealDto> PlannedMeals { get; set; } = new List<PlannedMealDto>();
}
