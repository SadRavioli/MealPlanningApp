using MealPlanner.Application.DTOs.ShoppingLists;
using MealPlanner.Domain.Entities;

namespace MealPlanner.Application.Mappers;

public static class ShoppingListMapper
{
    public static ShoppingListDto ToDto(ShoppingList shoppingList)
    {
        return new ShoppingListDto
        {
            Id = shoppingList.Id,
            HouseholdId = shoppingList.HouseholdId,
            MealPlanId = shoppingList.MealPlanId,
            CreatedAt = shoppingList.CreatedAt,
            Notes = shoppingList.Notes,
            Items = shoppingList.Items?.Select(ToItemDto).ToList()
                ?? new List<ShoppingListItemDto>()
        };
    }

    public static ShoppingListItemDto ToItemDto(ShoppingListItem item)
    {
        return new ShoppingListItemDto
        {
            Id = item.Id,
            IngredientId = item.IngredientId,
            IngredientName = item.Ingredient?.Name ?? string.Empty,
            Quantity = item.Quantity,
            Unit = item.Unit,
            IsChecked = item.IsChecked
        };
    }

    public static ShoppingList ToEntity(SaveShoppingListDto dto, int householdId)
    {
        return new ShoppingList
        {
            HouseholdId = householdId,
            MealPlanId = dto.MealPlanId,
            CreatedAt = DateTime.UtcNow,
            Notes = dto.Notes,
            Items = [.. dto.Items.Select(ToItemEntity)]
        };
    }

    public static ShoppingListItem ToItemEntity(SaveShoppingListItemDto dto)
    {
        return new ShoppingListItem
        {
            IngredientId = dto.IngredientId,
            Quantity = dto.Quantity,
            Unit = dto.Unit,
            IsChecked = false
        };
    }
}
