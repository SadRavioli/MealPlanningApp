namespace MealPlanner.Application.DTOs;

public class SaveRecipeDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public int ServingSize { get; set; }
    public List<SaveRecipeIngredientDto> Ingredients { get; set; } = new();
}

public class SaveRecipeIngredientDto
{
    public int IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public int Unit { get; set; }
    public string? Notes { get; set; }
}
