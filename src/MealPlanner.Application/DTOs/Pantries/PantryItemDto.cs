using MealPlanner.Domain.Enums;

namespace MealPlanner.Application.DTOs.Pantries;

public class PantryItemDto
{
    public int Id { get; set; }
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public MeasurementUnit Unit { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime AddedAt { get; set; }
}
