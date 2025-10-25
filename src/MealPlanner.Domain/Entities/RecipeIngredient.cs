using MealPlanner.Domain.Enums;
using MealPlanner.Domain.ValueObjects;

namespace MealPlanner.Domain.Entities;

public class RecipeIngredient
{
    public int RecipeId { get; set; }
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public MeasurementUnit Unit { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public Recipe Recipe { get; set; } = null!;
    public Ingredient Ingredient { get; set; } = null!;
}
