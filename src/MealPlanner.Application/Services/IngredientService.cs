using MealPlanner.Application.DTOs;
using MealPlanner.Application.Mappers;
using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Enums;
using MealPlanner.Domain.Repositories;

namespace MealPlanner.Application.Services;

public class IngredientService : IIngredientService
{
    private readonly IIngredientRepository _ingredientRepository;

    public IngredientService(IIngredientRepository ingredientRepository)
    {
        _ingredientRepository = ingredientRepository;
    }

    public async Task<IngredientDto?> GetIngredientByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var ingredient = await _ingredientRepository.GetByIdAsync(id, cancellationToken);
        return ingredient == null ? null : IngredientMapper.ToDto(ingredient);
    }

    public async Task<IEnumerable<IngredientDto>> GetAllIngredientsAsync(CancellationToken cancellationToken = default)
    {
        var ingredients = await _ingredientRepository.GetAllAsync(cancellationToken);
        return ingredients.Select(IngredientMapper.ToDto);
    }

    public async Task<IEnumerable<IngredientDto>> SearchIngredientsByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var ingredients = await _ingredientRepository.SearchByNameAsync(searchTerm, cancellationToken);
        return ingredients.Select(IngredientMapper.ToDto);
    }

    public async Task<IngredientDto> CreateIngredientAsync(SaveIngredientDto dto, CancellationToken cancellationToken = default)
    {
        var ingredient = IngredientMapper.ToEntity(dto);
        var created = await _ingredientRepository.AddAsync(ingredient, cancellationToken);
        return IngredientMapper.ToDto(created);
    }

    public async Task UpdateIngredientAsync(int id, SaveIngredientDto dto, CancellationToken cancellationToken = default)
    {
        var ingredient = await _ingredientRepository.GetByIdAsync(id, cancellationToken);
        if (ingredient == null)
            throw new KeyNotFoundException($"Ingredient with ID {id} not found");

        ingredient.Name = dto.Name;
        ingredient.Category = dto.Category;

        await _ingredientRepository.UpdateAsync(ingredient, cancellationToken);
    }

    public async Task DeleteIngredientAsync(int id, CancellationToken cancellationToken = default)
    {
        var ingredient = await _ingredientRepository.GetByIdAsync(id, cancellationToken);
        if (ingredient == null)
            throw new KeyNotFoundException($"Ingredient with ID {id} not found");

        await _ingredientRepository.DeleteAsync(ingredient, cancellationToken);
    }
}
