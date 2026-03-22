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
        var pantry = await _pantryRepository.GetByHouseholdIdWithItemsAsync(householdId, cancellationToken);
        return pantry == null ? new PantryDto() : PantryMapper.ToDto(pantry);
    }

    public async Task<PantryItemDto?> GetPantryItemByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var pantryItem = await _pantryRepository.GetPantryItemByIdAsync(id, cancellationToken);
        return pantryItem == null ? null : PantryMapper.ToItemDto(pantryItem);
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
        var pantry = await _pantryRepository.GetByHouseholdIdWithItemsAsync(householdId, cancellationToken);
        if (pantry == null)
            pantry = await _pantryRepository.AddAsync(new Pantry { HouseholdId = householdId }, cancellationToken);

        var existingItem = pantry.Items.FirstOrDefault(i => 
            i.IngredientId == dto.IngredientId && 
            i.ExpiryDate == dto.ExpiryDate);

        if (existingItem != null)
        {
            existingItem.Quantity += dto.Quantity;
            await _pantryRepository.UpdateAsync(pantry, cancellationToken);
            var itemDto = await GetPantryItemByIdAsync(existingItem.Id, cancellationToken)
                ?? throw new Exception("Failed to retrieve pantry item");
            itemDto.WasExisting = true;
            return itemDto;
        }

        var newItem = PantryMapper.ToItemEntity(dto, pantry.Id);
        pantry.Items.Add(newItem);
        await _pantryRepository.UpdateAsync(pantry, cancellationToken);
        return await GetPantryItemByIdAsync(newItem.Id, cancellationToken)
            ?? throw new Exception("Failed to retrieve newly added pantry item");
    }

    public async Task UpdatePantryItemAsync(int pantryId, int itemId, SavePantryItemDto dto, CancellationToken cancellationToken = default)
    {
        var pantry = await _pantryRepository.GetByHouseholdIdWithItemsAsync(pantryId, cancellationToken);
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

    public async Task RemoveItemFromPantryAsync(int itemId, CancellationToken cancellationToken = default)
    {
        var item = await _pantryRepository.GetPantryItemByIdAsync(itemId, cancellationToken);
        if (item == null)
            throw new KeyNotFoundException($"Pantry item with ID {itemId} not found");

        await _pantryRepository.RemovePantryItemAsync(item, cancellationToken);
    }
}
