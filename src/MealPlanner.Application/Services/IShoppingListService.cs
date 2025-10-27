using MealPlanner.Application.DTOs.ShoppingLists;

namespace MealPlanner.Application.Services;

public interface IShoppingListService
{
    Task<ShoppingListDto?> GetShoppingListByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ShoppingListDto>> GetShoppingListsByHouseholdAsync(int householdId, CancellationToken cancellationToken = default);
    Task<ShoppingListDto> CreateShoppingListAsync(SaveShoppingListDto dto, int householdId, CancellationToken cancellationToken = default);
    Task<ShoppingListDto> GenerateFromMealPlanAsync(int mealPlanId, int householdId, CancellationToken cancellationToken = default);
    Task UpdateShoppingListAsync(int id, SaveShoppingListDto dto, CancellationToken cancellationToken = default);
    Task DeleteShoppingListAsync(int id, CancellationToken cancellationToken = default);
    Task ToggleItemCheckedAsync(int shoppingListId, int itemId, CancellationToken cancellationToken = default);
}
