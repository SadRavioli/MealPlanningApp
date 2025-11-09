using MealPlanner.Application.DTOs.Recipes;
using MealPlanner.Application.Mappers;
using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Enums;
using MealPlanner.Domain.Repositories;

namespace MealPlanner.Application.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;

    public RecipeService(IRecipeRepository recipeRepository)
    {
        _recipeRepository = recipeRepository;
    }

    public async Task<RecipeDto?> GetRecipeByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var recipe = await _recipeRepository.GetByIdWithIngredientsAsync(id, cancellationToken);
        return recipe == null ? null : RecipeMapper.ToDto(recipe);
    }

    public async Task<IEnumerable<RecipeDto>> GetRecipesByHouseholdAsync(int householdId, CancellationToken cancellationToken = default)
    {
        var recipes = await _recipeRepository.GetByHouseholdIdAsync(householdId, cancellationToken);
        return recipes.Select(RecipeMapper.ToDto);
    }

    public async Task<RecipeDto> CreateRecipeAsync(int householdId, SaveRecipeDto dto, CancellationToken cancellationToken = default)
    {
        var recipe = RecipeMapper.ToEntity(dto, householdId);
        var created = await _recipeRepository.AddAsync(recipe, cancellationToken);
        return RecipeMapper.ToDto(created);
    }

    public async Task UpdateRecipeAsync(int id, SaveRecipeDto dto, CancellationToken cancellationToken = default)
    {
        var recipe = await _recipeRepository.GetByIdWithIngredientsAsync(id, cancellationToken);
        if (recipe == null)
            throw new KeyNotFoundException($"Recipe with ID {id} not found");

        // Update recipe properties
        recipe.Name = dto.Name;
        recipe.Description = dto.Description;
        recipe.Instructions = dto.Instructions;
        recipe.PrepTimeMinutes = dto.PrepTimeMinutes;
        recipe.CookTimeMinutes = dto.CookTimeMinutes;
        recipe.ServingSize = dto.ServingSize;
        recipe.UpdatedAt = DateTime.UtcNow;

        // Update ingredients using Remove/Update/Add pattern
        // 1. Remove ingredients that are no longer in the DTO
        var dtoIngredientIds = dto.Ingredients.Select(i => i.IngredientId).ToHashSet();
        var ingredientsToRemove = recipe.RecipeIngredients
            .Where(ri => !dtoIngredientIds.Contains(ri.IngredientId))
            .ToList();

        foreach (var ingredient in ingredientsToRemove)
        {
            recipe.RecipeIngredients.Remove(ingredient);
        }

        // 2. Update existing ingredients or add new ones
        foreach (var dtoIngredient in dto.Ingredients)
        {
            var existingIngredient = recipe.RecipeIngredients
                .FirstOrDefault(ri => ri.IngredientId == dtoIngredient.IngredientId);

            if (existingIngredient != null)
            {
                // Update existing ingredient
                existingIngredient.Quantity = dtoIngredient.Quantity;
                existingIngredient.Unit = (MeasurementUnit)dtoIngredient.Unit;
                existingIngredient.Notes = dtoIngredient.Notes;
            }
            else
            {
                // Add new ingredient
                recipe.RecipeIngredients.Add(new RecipeIngredient
                {
                    RecipeId = recipe.Id,
                    IngredientId = dtoIngredient.IngredientId,
                    Quantity = dtoIngredient.Quantity,
                    Unit = (MeasurementUnit)dtoIngredient.Unit,
                    Notes = dtoIngredient.Notes
                });
            }
        }

        await _recipeRepository.UpdateAsync(recipe, cancellationToken);
    }

    public async Task DeleteRecipeAsync(int id, CancellationToken cancellationToken = default)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id, cancellationToken);
        if (recipe == null)
            throw new KeyNotFoundException($"Recipe with ID {id} not found");

        await _recipeRepository.DeleteAsync(recipe, cancellationToken);
    }

    public async Task<RecipeDto> ScaleRecipeAsync(int recipeId, int newServings, CancellationToken cancellationToken = default)
    {
        if (newServings <= 0)
            throw new ArgumentException("New servings must be greater than zero", nameof(newServings));

        // Get recipe with ingredients
        var recipe = await _recipeRepository.GetByIdWithIngredientsAsync(recipeId, cancellationToken);
        if (recipe == null)
            throw new KeyNotFoundException($"Recipe with ID {recipeId} not found");

        // Calculate scale factor
        var scaleFactor = (decimal)newServings / recipe.ServingSize;

        // Map to DTO and scale ingredients
        var recipeDto = RecipeMapper.ToDto(recipe);
        recipeDto.ServingSize = newServings;

        foreach (var ingredient in recipeDto.Ingredients)
        {
            ingredient.Quantity *= scaleFactor;
        }

        return recipeDto;
    }

    public async Task<IEnumerable<RecipeDto>> SearchRecipesAsync(int householdId, string searchTerm, CancellationToken cancellationToken = default)
    {
        var recipes = await _recipeRepository.SearchByNameAsync(householdId, searchTerm, cancellationToken);
        return recipes.Select(RecipeMapper.ToDto);
    }
}
