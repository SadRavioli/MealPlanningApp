using MealPlanner.Domain.Enums;

namespace MealPlanner.Application.DTOs.Households;

public class HouseholdMemberDto
{
    public string UserId { get; set; } = string.Empty;
    public HouseholdRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
}
