using MealPlanner.Application.DTOs.Households;
using MealPlanner.Domain.Enums;

namespace MealPlanner.Application.Services;

public interface IHouseholdService
{
    Task<HouseholdDto?> GetHouseholdByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<HouseholdDto>> GetHouseholdsByUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<HouseholdDto> CreateHouseholdAsync(SaveHouseholdDto dto, CancellationToken cancellationToken = default);
    Task UpdateHouseholdAsync(int id, SaveHouseholdDto dto, CancellationToken cancellationToken = default);
    Task DeleteHouseholdAsync(int id, CancellationToken cancellationToken = default);
    Task AddMemberToHouseholdAsync(int householdId, HouseholdMemberDto memberDto, CancellationToken cancellationToken = default);
    Task RemoveMemberFromHouseholdAsync(int householdId, string userId, CancellationToken cancellationToken = default);
    Task UpdateHouseholdMemberRoleAsync(int householdId, string userId, HouseholdRole newRole, CancellationToken cancellationToken = default);
}
