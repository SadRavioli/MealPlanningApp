namespace MealPlanner.Domain.Entities;

public class Pantry
{
    public int Id { get; set; }
    public int HouseholdId { get; set; }

    // Navigation properties
    public Household Household { get; set; } = null!;
    public ICollection<PantryItem> Items { get; set; } = new List<PantryItem>();
}
