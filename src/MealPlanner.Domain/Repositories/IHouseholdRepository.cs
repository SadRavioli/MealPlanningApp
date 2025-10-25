using MealPlanner.Domain.Entities;

namespace MealPlanner.Domain.Repositories;

public interface IHouseholdRepository : IRepository<Household>
{
    Task<IEnumerable<Household>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Household?> GetByIdWithMembersAsync(int id, CancellationToken cancellationToken = default);
}
