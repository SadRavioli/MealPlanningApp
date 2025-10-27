using MealPlanner.Application.DTOs.Pantries;
using MealPlanner.Domain.Entities;

namespace MealPlanner.Application.Mappers;

public static class PantryMapper
{
    public static PantryDto ToDto(Pantry pantry)
    {
        return new PantryDto
        {
            Id = pantry.Id,
            HouseholdId = pantry.HouseholdId,
            Items = pantry.Items?.Select(ToItemDto).ToList()
                ?? new List<PantryItemDto>()
        };
    }

    public static PantryItemDto ToItemDto(PantryItem item)
    {
        return new PantryItemDto
        {
            Id = item.Id,
            IngredientId = item.IngredientId,
            IngredientName = item.Ingredient?.Name ?? string.Empty,
            Quantity = item.Quantity,
            Unit = item.Unit,
            ExpiryDate = item.ExpiryDate,
            AddedAt = item.AddedAt
        };
    }

    public static PantryItem ToItemEntity(SavePantryItemDto dto, int pantryId)
    {
        return new PantryItem
        {
            PantryId = pantryId,
            IngredientId = dto.IngredientId,
            Quantity = dto.Quantity,
            Unit = dto.Unit,
            ExpiryDate = dto.ExpiryDate,
            AddedAt = DateTime.UtcNow
        };
    }
}
