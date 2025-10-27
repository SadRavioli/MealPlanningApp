using MealPlanner.Domain.Enums;

namespace MealPlanner.Application.DTOs.Recipes;

public class RecipeIngredientDto
{
    public int IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public MeasurementUnit Unit { get; set; }
    public string? Notes { get; set; }
}
