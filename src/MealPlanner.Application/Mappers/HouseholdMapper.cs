using MealPlanner.Application.DTOs.Households;
using MealPlanner.Domain.Entities;

namespace MealPlanner.Application.Mappers;

public static class HouseholdMapper
{
    public static HouseholdDto ToDto(Household household)
    {
        return new HouseholdDto
        {
            Id = household.Id,
            Name = household.Name,
            CreatedAt = household.CreatedAt,
            Members = household.UserHouseholds?.Select(ToMemberDto).ToList()
                ?? new List<HouseholdMemberDto>()
        };
    }

    public static HouseholdMemberDto ToMemberDto(UserHousehold userHousehold)
    {
        return new HouseholdMemberDto
        {
            UserId = userHousehold.UserId,
            Role = userHousehold.Role,
            JoinedAt = userHousehold.JoinedAt
        };
    }

    public static Household ToEntity(SaveHouseholdDto dto)
    {
        return new Household
        {
            Name = dto.Name,
            CreatedAt = DateTime.UtcNow
        };
    }
}
