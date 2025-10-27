using MealPlanner.Domain.Enums;

namespace MealPlanner.Application.DTOs.ShoppingLists;

public class ShoppingListItemDto
{
    public int Id { get; set; }
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public MeasurementUnit Unit { get; set; }
    public bool IsChecked { get; set; }
}
