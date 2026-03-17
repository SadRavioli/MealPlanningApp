using MealPlanner.Domain.Entities;

namespace MealPlanner.Domain.Repositories;

public interface IShoppingListRepository : IRepository<ShoppingList>
{
    Task<IReadOnlyList<ShoppingList>> GetByHouseholdIdAsync(int householdId, CancellationToken cancellationToken = default);
    Task<ShoppingList?> GetByIdWithItemsAsync(int id, CancellationToken cancellationToken = default);
    Task<ShoppingList?> GetActiveListByHouseholdAsync(int householdId, CancellationToken cancellationToken = default);
    Task<ShoppingListItem?> GetListItemByIdAsync(int id, CancellationToken cancellationToken = default);
    Task UpdateItemAsync(ShoppingListItem item, CancellationToken cancellationToken = default);
}
