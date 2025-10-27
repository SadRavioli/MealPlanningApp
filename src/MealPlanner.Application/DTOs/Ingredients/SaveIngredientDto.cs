namespace MealPlanner.Application.DTOs.Ingredients;

public class SaveIngredientDto 
{
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
}
