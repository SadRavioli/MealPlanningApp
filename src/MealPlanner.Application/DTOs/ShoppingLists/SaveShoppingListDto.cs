namespace MealPlanner.Application.DTOs.ShoppingLists;

public class SaveShoppingListDto
{
    public int? MealPlanId { get; set; }
    public string? Notes { get; set; }
    public IEnumerable<SaveShoppingListItemDto> Items { get; set; } = new List<SaveShoppingListItemDto>();
}
