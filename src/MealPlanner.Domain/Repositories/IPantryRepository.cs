using MealPlanner.Domain.Entities;

namespace MealPlanner.Domain.Repositories;

public interface IPantryRepository : IRepository<Pantry>
{
    Task<Pantry?> GetByHouseholdIdAsync(int householdId, CancellationToken cancellationToken = default);
    Task<Pantry?> GetByIdWithItemsAsync(int id, CancellationToken cancellationToken = default);
}
