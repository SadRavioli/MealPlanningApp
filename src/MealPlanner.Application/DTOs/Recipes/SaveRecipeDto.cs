namespace MealPlanner.Application.DTOs.Recipes;

public class SaveRecipeDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public int ServingSize { get; set; }
    public IEnumerable<SaveRecipeIngredientDto> Ingredients { get; set; } = new List<SaveRecipeIngredientDto>();
}
