using MealPlanner.Application.DTOs.Pantries;

namespace MealPlanner.Application.Services;

public interface IPantryService
{
    Task<PantryDto?> GetPantryByHouseholdIdAsync(int householdId, CancellationToken cancellationToken = default);
    Task<PantryItemDto?> GetPantryItemByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PantryDto> CreatePantryAsync(int householdId, CancellationToken cancellationToken = default);
    Task<PantryItemDto> AddItemToPantryAsync(int householdId, SavePantryItemDto dto, CancellationToken cancellationToken = default);
    Task UpdatePantryItemAsync(int pantryId, int itemId, SavePantryItemDto dto, CancellationToken cancellationToken = default);
    Task RemoveItemFromPantryAsync(int itemId, CancellationToken cancellationToken = default);
}
