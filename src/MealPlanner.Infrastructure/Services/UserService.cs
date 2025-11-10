using MealPlanner.Application.DTOs.Users;
using MealPlanner.Application.Services;
using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Enums;
using MealPlanner.Domain.Repositories;
using MealPlanner.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace MealPlanner.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHouseholdRepository _householdRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserManager<ApplicationUser> userManager,
        IHouseholdRepository householdRepository,
        ILogger<UserService> logger)
    {
        _userManager = userManager;
        _householdRepository = householdRepository;
        _logger = logger;
    }

    public async Task<(bool Succeeded, UserDto? User, IEnumerable<string> Errors)> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        // Create the user
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return (false, null, result.Errors.Select(e => e.Description));
        }

        _logger.LogInformation("User {Email} created successfully", dto.Email);

        // Create default household for the user
        var household = new Household
        {
            Name = $"{dto.FirstName}'s Household",
            CreatedAt = DateTime.UtcNow
        };

        household = await _householdRepository.AddAsync(household, cancellationToken);

        // Create UserHousehold relationship
        var userHousehold = new UserHousehold
        {
            UserId = user.Id,
            HouseholdId = household.Id,
            Role = HouseholdRole.Admin,
            JoinedAt = DateTime.UtcNow
        };

        household.UserHouseholds.Add(userHousehold);
        await _householdRepository.UpdateAsync(household, cancellationToken);

        _logger.LogInformation("Default household {HouseholdId} created for user {UserId}", household.Id, user.Id);

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            HouseholdId = household.Id
        };

        return (true, userDto, Array.Empty<string>());
    }

    public async Task<(bool Succeeded, UserDto? User)> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        // Verify user credentials (PageModel will handle actual sign-in with SignInManager)
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            _logger.LogWarning("User not found for login: {Email}", dto.Email);
            return (false, null);
        }

        // Check password
        var isValidPassword = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isValidPassword)
        {
            _logger.LogWarning("Invalid password for user: {Email}", dto.Email);
            return (false, null);
        }

        _logger.LogInformation("User credentials verified for {Email}", dto.Email);

        // Get user's household
        var households = await _householdRepository.GetByUserIdAsync(user.Id, cancellationToken);
        var household = households.FirstOrDefault();

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            HouseholdId = household?.Id
        };

        return (true, userDto);
    }

    public Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        // Sign-out is handled by PageModel with SignInManager
        _logger.LogInformation("Logout requested");
        return Task.CompletedTask;
    }

    public async Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return null;
        }

        // Get user's household (first one for MVP - users can have multiple)
        var households = await _householdRepository.GetByUserIdAsync(userId, cancellationToken);
        var household = households.FirstOrDefault();

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            HouseholdId = household?.Id
        };
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return null;
        }

        // Get user's household (first one for MVP - users can have multiple)
        var households = await _householdRepository.GetByUserIdAsync(user.Id, cancellationToken);
        var household = households.FirstOrDefault();

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            HouseholdId = household?.Id
        };
    }
}
