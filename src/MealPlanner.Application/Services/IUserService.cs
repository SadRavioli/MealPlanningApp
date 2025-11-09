using MealPlanner.Application.DTOs.Users;

namespace MealPlanner.Application.Services;

public interface IUserService
{
    Task<(bool Succeeded, UserDto? User, IEnumerable<string> Errors)> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default);
    Task<(bool Succeeded, UserDto? User)> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
    Task LogoutAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
}
