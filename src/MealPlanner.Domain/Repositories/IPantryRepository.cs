using MealPlanner.Domain.Entities;

namespace MealPlanner.Domain.Repositories;

public interface IPantryRepository : IRepository<Pantry>
{
    Task<Pantry?> GetByHouseholdIdWithItemsAsync(int householdId, CancellationToken cancellationToken = default);
    Task<PantryItem?> GetPantryItemByIdAsync(int id, CancellationToken cancellationToken = default);
    Task RemovePantryItemAsync(PantryItem entity, CancellationToken cancellationToken = default);
}
