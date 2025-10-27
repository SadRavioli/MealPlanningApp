using MealPlanner.Application.DTOs.Recipes;
using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Enums;

namespace MealPlanner.Application.Mappers;

public static class RecipeMapper
{
    public static RecipeDto ToDto(Recipe recipe)
    {
        return new RecipeDto
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Description = recipe.Description,
            Instructions = recipe.Instructions,
            PrepTimeMinutes = recipe.PrepTimeMinutes,
            CookTimeMinutes = recipe.CookTimeMinutes,
            ServingSize = recipe.ServingSize,
            HouseholdId = recipe.HouseholdId,
            CreatedAt = recipe.CreatedAt,
            UpdatedAt = recipe.UpdatedAt,
            Ingredients = recipe.RecipeIngredients?.Select(ToIngredientDto).ToList()
                ?? new List<RecipeIngredientDto>()
        };
    }

    public static RecipeIngredientDto ToIngredientDto(RecipeIngredient recipeIngredient)
    {
        return new RecipeIngredientDto
        {
            IngredientId = recipeIngredient.IngredientId,
            IngredientName = recipeIngredient.Ingredient?.Name ?? string.Empty,
            Quantity = recipeIngredient.Quantity,
            Unit = recipeIngredient.Unit,
            Notes = recipeIngredient.Notes
        };
    }

    public static Recipe ToEntity(SaveRecipeDto dto, int householdId)
    {
        return new Recipe
        {
            Name = dto.Name,
            Description = dto.Description,
            Instructions = dto.Instructions,
            PrepTimeMinutes = dto.PrepTimeMinutes,
            CookTimeMinutes = dto.CookTimeMinutes,
            ServingSize = dto.ServingSize,
            HouseholdId = householdId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            RecipeIngredients = [.. dto.Ingredients.Select(ToIngredientEntity)]
        };
    }

    public static RecipeIngredient ToIngredientEntity(SaveRecipeIngredientDto dto)
    {
        return new RecipeIngredient
        {
            IngredientId = dto.IngredientId,
            Quantity = dto.Quantity,
            Unit = (MeasurementUnit)dto.Unit,
            Notes = dto.Notes
        };
    }
}
