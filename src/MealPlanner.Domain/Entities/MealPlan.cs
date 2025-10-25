namespace MealPlanner.Domain.Entities;

public class MealPlan
{
    public int Id { get; set; }
    public int HouseholdId { get; set; }
    public DateTime WeekStartDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Household Household { get; set; } = null!;
    public ICollection<PlannedMeal> PlannedMeals { get; set; } = new List<PlannedMeal>();
}
