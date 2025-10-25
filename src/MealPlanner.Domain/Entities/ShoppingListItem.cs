using MealPlanner.Domain.Enums;

namespace MealPlanner.Domain.Entities;

public class ShoppingListItem
{
    public int Id { get; set; }
    public int ShoppingListId { get; set; }
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public MeasurementUnit Unit { get; set; }
    public bool IsChecked { get; set; }

    // Navigation properties
    public ShoppingList ShoppingList { get; set; } = null!;
    public Ingredient Ingredient { get; set; } = null!;
}
