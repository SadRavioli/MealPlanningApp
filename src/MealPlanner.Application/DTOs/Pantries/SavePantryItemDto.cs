using MealPlanner.Domain.Enums;

namespace MealPlanner.Application.DTOs.Pantries;

public class SavePantryItemDto
{
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public MeasurementUnit Unit { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
