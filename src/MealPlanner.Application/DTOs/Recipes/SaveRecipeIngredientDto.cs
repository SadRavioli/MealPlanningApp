namespace MealPlanner.Application.DTOs.Recipes;

public class SaveRecipeIngredientDto
{
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public int Unit { get; set; }
    public string? Notes { get; set; }
}
