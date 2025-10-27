namespace MealPlanner.Application.DTOs.Pantries;

public class PantryDto
{
    public int Id { get; set; }
    public int HouseholdId { get; set; }
    public IEnumerable<PantryItemDto> Items { get; set; } = new List<PantryItemDto>();
}
