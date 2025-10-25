using MealPlanner.Application.DTOs;

namespace MealPlanner.Application.Services;

public interface IRecipeService
{
    Task<RecipeDto?> GetRecipeByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<RecipeDto>> GetRecipesByHouseholdAsync(int householdId, CancellationToken cancellationToken = default);
    Task<RecipeDto> CreateRecipeAsync(int householdId, SaveRecipeDto dto, CancellationToken cancellationToken = default);
    Task UpdateRecipeAsync(int id, SaveRecipeDto dto, CancellationToken cancellationToken = default);
    Task DeleteRecipeAsync(int id, CancellationToken cancellationToken = default);
    Task<RecipeDto> ScaleRecipeAsync(int recipeId, int newServings, CancellationToken cancellationToken = default);
    Task<IEnumerable<RecipeDto>> SearchRecipesAsync(int householdId, string searchTerm, CancellationToken cancellationToken = default);
}
