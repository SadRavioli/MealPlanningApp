namespace MealPlanner.Application.DTOs;

public class SaveRecipeIngredientDto
{
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public int Unit { get; set; }
    public string? Notes { get; set; }
}
