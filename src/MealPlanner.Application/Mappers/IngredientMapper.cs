using MealPlanner.Application.DTOs;
using MealPlanner.Domain.Entities;

namespace MealPlanner.Application.Mappers;

public static class IngredientMapper
{
    public static IngredientDto ToDto(Ingredient ingredient)
    {
        return new IngredientDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Category = ingredient.Category,
            RecipeIngredientDtos = ingredient.RecipeIngredients?.Select(RecipeMapper.ToIngredientDto).ToList()
                ?? new List<RecipeIngredientDto>()
        };
    }

    public static Ingredient ToEntity(SaveIngredientDto dto)
    {
        return new Ingredient
        {
            Name = dto.Name,
            Category = dto.Category
        };
    }
}
