namespace MealPlanner.Application.DTOs.ShoppingLists;

public class ShoppingListDto
{
    public int Id { get; set; }
    public int HouseholdId { get; set; }
    public int? MealPlanId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Notes { get; set; }
    public IEnumerable<ShoppingListItemDto> Items { get; set; } = new List<ShoppingListItemDto>();
}
