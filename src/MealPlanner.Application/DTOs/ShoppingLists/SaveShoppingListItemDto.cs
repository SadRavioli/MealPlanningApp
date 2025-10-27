using MealPlanner.Domain.Enums;

namespace MealPlanner.Application.DTOs.ShoppingLists;

public class SaveShoppingListItemDto
{
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public MeasurementUnit Unit { get; set; }
}
