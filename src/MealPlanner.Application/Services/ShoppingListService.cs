using MealPlanner.Application.DTOs.ShoppingLists;
using MealPlanner.Application.Mappers;
using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Repositories;

namespace MealPlanner.Application.Services;

public class ShoppingListService : IShoppingListService
{
    private readonly IShoppingListRepository _shoppingListRepository;
    private readonly IMealPlanRepository _mealPlanRepository;

    public ShoppingListService(
        IShoppingListRepository shoppingListRepository,
        IMealPlanRepository mealPlanRepository)
    {
        _shoppingListRepository = shoppingListRepository;
        _mealPlanRepository = mealPlanRepository;
    }

    public async Task<ShoppingListDto?> GetShoppingListByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var shoppingList = await _shoppingListRepository.GetByIdWithItemsAsync(id, cancellationToken);
        return shoppingList == null ? null : ShoppingListMapper.ToDto(shoppingList);
    }

    public async Task<IEnumerable<ShoppingListDto>> GetShoppingListsByHouseholdAsync(int householdId, CancellationToken cancellationToken = default)
    {
        var shoppingLists = await _shoppingListRepository.GetByHouseholdIdAsync(householdId, cancellationToken);
        return shoppingLists.Select(ShoppingListMapper.ToDto);
    }

    public async Task<ShoppingListDto> CreateShoppingListAsync(SaveShoppingListDto dto, int householdId, CancellationToken cancellationToken = default)
    {
        var shoppingList = ShoppingListMapper.ToEntity(dto, householdId);
        var created = await _shoppingListRepository.AddAsync(shoppingList, cancellationToken);
        return ShoppingListMapper.ToDto(created);
    }

    public async Task<ShoppingListDto> GenerateFromMealPlanAsync(int mealPlanId, int householdId, CancellationToken cancellationToken = default)
    {
        var mealPlan = await _mealPlanRepository.GetByIdWithIngredientsAsync(mealPlanId, cancellationToken);
        if (mealPlan == null)
            throw new KeyNotFoundException($"Meal plan with ID {mealPlanId} not found");

        // Aggregate ingredients from all recipes in the meal plan
        var aggregatedIngredients = new Dictionary<(int IngredientId, string Unit), decimal>();

        foreach (var plannedMeal in mealPlan.PlannedMeals)
        {
            if (plannedMeal.Recipe?.RecipeIngredients == null)
                continue;

            foreach (var recipeIngredient in plannedMeal.Recipe.RecipeIngredients)
            {
                // Scale quantity by servings planned
                var scaleFactor = (decimal)plannedMeal.Servings / plannedMeal.Recipe.ServingSize;
                var scaledQuantity = recipeIngredient.Quantity * scaleFactor;

                var key = (recipeIngredient.IngredientId, recipeIngredient.Unit.ToString());

                if (aggregatedIngredients.ContainsKey(key))
                    aggregatedIngredients[key] += scaledQuantity;
                else
                    aggregatedIngredients[key] = scaledQuantity;
            }
        }

        // Create shopping list with aggregated items
        var shoppingList = new ShoppingList
        {
            HouseholdId = householdId,
            MealPlanId = mealPlanId,
            CreatedAt = DateTime.UtcNow,
            Notes = $"Generated from meal plan for week of {mealPlan.WeekStartDate:yyyy-MM-dd}",
            Items = aggregatedIngredients.Select(kvp => new ShoppingListItem
            {
                IngredientId = kvp.Key.IngredientId,
                Quantity = kvp.Value,
                Unit = Enum.Parse<Domain.Enums.MeasurementUnit>(kvp.Key.Unit),
                IsChecked = false
            }).ToList()
        };

        var created = await _shoppingListRepository.AddAsync(shoppingList, cancellationToken);
        return ShoppingListMapper.ToDto(created);
    }

    public async Task UpdateShoppingListAsync(int id, SaveShoppingListDto dto, CancellationToken cancellationToken = default)
    {
        var shoppingList = await _shoppingListRepository.GetByIdWithItemsAsync(id, cancellationToken);
        if (shoppingList == null)
            throw new KeyNotFoundException($"Shopping list with ID {id} not found");

        shoppingList.MealPlanId = dto.MealPlanId;
        shoppingList.Notes = dto.Notes;

        // Update items - clear and replace
        shoppingList.Items.Clear();
        foreach (var itemDto in dto.Items)
        {
            shoppingList.Items.Add(ShoppingListMapper.ToItemEntity(itemDto));
        }

        await _shoppingListRepository.UpdateAsync(shoppingList, cancellationToken);
    }

    public async Task DeleteShoppingListAsync(int id, CancellationToken cancellationToken = default)
    {
        var shoppingList = await _shoppingListRepository.GetByIdAsync(id, cancellationToken);
        if (shoppingList == null)
            throw new KeyNotFoundException($"Shopping list with ID {id} not found");

        await _shoppingListRepository.DeleteAsync(shoppingList, cancellationToken);
    }

    public async Task ToggleItemCheckedAsync(int shoppingListId, int itemId, CancellationToken cancellationToken = default)
    {
        var shoppingList = await _shoppingListRepository.GetByIdWithItemsAsync(shoppingListId, cancellationToken);
        if (shoppingList == null)
            throw new KeyNotFoundException($"Shopping list with ID {shoppingListId} not found");

        var item = shoppingList.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new KeyNotFoundException($"Shopping list item with ID {itemId} not found");

        item.IsChecked = !item.IsChecked;
        await _shoppingListRepository.UpdateAsync(shoppingList, cancellationToken);
    }
}
