using MealPlanner.Application.DTOs.Households;
using MealPlanner.Application.Mappers;
using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Enums;
using MealPlanner.Domain.Repositories;

namespace MealPlanner.Application.Services;

public class HouseholdService : IHouseholdService
{
    private readonly IHouseholdRepository _householdRepository;

    public HouseholdService(IHouseholdRepository householdRepository)
    {
        _householdRepository = householdRepository;
    }

    public async Task<HouseholdDto?> GetHouseholdByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var household = await _householdRepository.GetByIdAsync(id, cancellationToken);
        return household == null ? null : HouseholdMapper.ToDto(household);
    }

    public async Task<IEnumerable<HouseholdDto>> GetHouseholdsByUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var households = await _householdRepository.GetByUserIdAsync(userId, cancellationToken);
        return households.Select(HouseholdMapper.ToDto);
    }

    public async Task<HouseholdDto> CreateHouseholdAsync(SaveHouseholdDto dto, CancellationToken cancellationToken = default)
    {
        var household = HouseholdMapper.ToEntity(dto);
        var created = await _householdRepository.AddAsync(household, cancellationToken);
        return HouseholdMapper.ToDto(created);
    }

    public async Task UpdateHouseholdAsync(int id, SaveHouseholdDto dto, CancellationToken cancellationToken = default)
    {
        var household = await _householdRepository.GetByIdAsync(id, cancellationToken);
        if (household == null)
            throw new KeyNotFoundException($"Household with ID {id} not found");

        household.Name = dto.Name;

        await _householdRepository.UpdateAsync(household, cancellationToken);
    }

    public async Task DeleteHouseholdAsync(int id, CancellationToken cancellationToken = default)
    {
        var household = await _householdRepository.GetByIdAsync(id, cancellationToken);
        if (household == null)
            throw new KeyNotFoundException($"Household with ID {id} not found");

        await _householdRepository.DeleteAsync(household, cancellationToken);
    }

    public async Task AddMemberToHouseholdAsync(int householdId, HouseholdMemberDto memberDto, CancellationToken cancellationToken = default)
    {
        var household = await _householdRepository.GetByIdWithMembersAsync(householdId, cancellationToken);
        if (household == null)
            throw new KeyNotFoundException($"Household with ID {householdId} not found");

        var userHousehold = new UserHousehold
        {
            UserId = memberDto.UserId,
            HouseholdId = householdId,
            Role = memberDto.Role,
            JoinedAt = DateTime.UtcNow
        };

        household.UserHouseholds.Add(userHousehold);
        await _householdRepository.UpdateAsync(household, cancellationToken);
    }

    public async Task RemoveMemberFromHouseholdAsync(int householdId, string userId, CancellationToken cancellationToken = default)
    {
        var household = await _householdRepository.GetByIdWithMembersAsync(householdId, cancellationToken);
        if (household == null)
            throw new KeyNotFoundException($"Household with ID {householdId} not found");

        var member = household.UserHouseholds.FirstOrDefault(uh => uh.UserId == userId);
        if (member == null)
            throw new KeyNotFoundException($"User with ID {userId} is not a member of household {householdId}");

        household.UserHouseholds.Remove(member);
        await _householdRepository.UpdateAsync(household, cancellationToken);
    }

    public async Task UpdateHouseholdMemberRoleAsync(int householdId, string userId, HouseholdRole newRole, CancellationToken cancellationToken = default)
    {
        var household = await _householdRepository.GetByIdWithMembersAsync(householdId, cancellationToken);
        if (household == null)
            throw new KeyNotFoundException($"Household with ID {householdId} not found");

        var member = household.UserHouseholds.FirstOrDefault(uh => uh.UserId == userId);
        if (member == null)
            throw new KeyNotFoundException($"User with ID {userId} is not a member of household {householdId}");

        member.Role = newRole;
        await _householdRepository.UpdateAsync(household, cancellationToken);
    }
}