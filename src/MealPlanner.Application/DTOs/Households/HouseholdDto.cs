namespace MealPlanner.Application.DTOs.Households;

public class HouseholdDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public IEnumerable<HouseholdMemberDto> Members { get; set; } = new List<HouseholdMemberDto>();
}
