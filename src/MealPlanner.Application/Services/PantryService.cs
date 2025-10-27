using MealPlanner.Application.DTOs.Pantries;
using MealPlanner.Application.Mappers;
using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Repositories;

namespace MealPlanner.Application.Services;

public class PantryService : IPantryService
{
    private readonly IPantryRepository _pantryRepository;

    public PantryService(IPantryRepository pantryRepository)
    {
        _pantryRepository = pantryRepository;
    }

    public async Task<PantryDto?> GetPantryByHouseholdIdAsync(int householdId, CancellationToken cancellationToken = default)
    {
        var pantry = await _pantryRepository.GetByHouseholdIdAsync(householdId, cancellationToken);
        return pantry == null ? null : PantryMapper.ToDto(pantry);
    }

    public async Task<PantryDto> CreatePantryAsync(int householdId, CancellationToken cancellationToken = default)
    {
        var pantry = new Pantry
        {
            HouseholdId = householdId
        };
        var created = await _pantryRepository.AddAsync(pantry, cancellationToken);
        return PantryMapper.ToDto(created);
    }

    public async Task<PantryItemDto> AddItemToPantryAsync(int householdId, SavePantryItemDto dto, CancellationToken cancellationToken = default)
    {
        // Get or create pantry using the separate methods
        var pantryDto = await GetPantryByHouseholdIdAsync(householdId, cancellationToken);
        pantryDto ??= await CreatePantryAsync(householdId, cancellationToken);

        var pantry = await _pantryRepository.GetByIdWithItemsAsync(pantryDto.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Pantry for household {householdId} not found");

        var newItem = PantryMapper.ToItemEntity(dto, pantry.Id);
        pantry.Items.Add(newItem);

        await _pantryRepository.UpdateAsync(pantry, cancellationToken);

        return PantryMapper.ToItemDto(newItem);
    }

    public async Task UpdatePantryItemAsync(int pantryId, int itemId, SavePantryItemDto dto, CancellationToken cancellationToken = default)
    {
        var pantry = await _pantryRepository.GetByIdWithItemsAsync(pantryId, cancellationToken);
        if (pantry == null)
            throw new KeyNotFoundException($"Pantry with ID {pantryId} not found");

        var item = pantry.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new KeyNotFoundException($"Pantry item with ID {itemId} not found");

        item.IngredientId = dto.IngredientId;
        item.Quantity = dto.Quantity;
        item.Unit = dto.Unit;
        item.ExpiryDate = dto.ExpiryDate;

        await _pantryRepository.UpdateAsync(pantry, cancellationToken);
    }

    public async Task RemoveItemFromPantryAsync(int pantryId, int itemId, CancellationToken cancellationToken = default)
    {
        var pantry = await _pantryRepository.GetByIdWithItemsAsync(pantryId, cancellationToken);
        if (pantry == null)
            throw new KeyNotFoundException($"Pantry with ID {pantryId} not found");

        var item = pantry.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new KeyNotFoundException($"Pantry item with ID {itemId} not found");

        pantry.Items.Remove(item);
        await _pantryRepository.UpdateAsync(pantry, cancellationToken);
    }
}
