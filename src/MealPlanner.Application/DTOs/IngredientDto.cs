namespace MealPlanner.Application.DTOs;

public class IngredientDto 
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public IEnumerable<RecipeIngredientDto> RecipeIngredientDtos { get; set; } = new List<RecipeIngredientDto>();
}
