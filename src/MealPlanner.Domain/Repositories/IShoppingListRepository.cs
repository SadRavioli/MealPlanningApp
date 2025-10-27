using MealPlanner.Domain.Entities;

namespace MealPlanner.Domain.Repositories;

public interface IShoppingListRepository : IRepository<ShoppingList>
{
    Task<IReadOnlyList<ShoppingList>> GetByHouseholdIdAsync(int householdId, CancellationToken cancellationToken = default);
    Task<ShoppingList?> GetByIdWithItemsAsync(int id, CancellationToken cancellationToken = default);
}
