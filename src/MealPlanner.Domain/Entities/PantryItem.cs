using MealPlanner.Domain.Enums;

namespace MealPlanner.Domain.Entities;

public class PantryItem
{
    public int Id { get; set; }
    public int PantryId { get; set; }
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public MeasurementUnit Unit { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Pantry Pantry { get; set; } = null!;
    public Ingredient Ingredient { get; set; } = null!;
}
