namespace MealPlanner.Domain.Entities;

public class ShoppingList
{
    public int Id { get; set; }
    public int HouseholdId { get; set; }
    public int? MealPlanId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }

    // Navigation properties
    public Household Household { get; set; } = null!;
    public MealPlan? MealPlan { get; set; }
    public ICollection<ShoppingListItem> Items { get; set; } = new List<ShoppingListItem>();
}
