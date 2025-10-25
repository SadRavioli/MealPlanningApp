using MealPlanner.Domain.Enums;

namespace MealPlanner.Domain.Entities;

public class UserHousehold
{
    public string UserId { get; set; } = string.Empty;
    public int HouseholdId { get; set; }
    public HouseholdRole Role { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Household Household { get; set; } = null!;
}
