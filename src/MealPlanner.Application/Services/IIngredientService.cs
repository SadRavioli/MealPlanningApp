using MealPlanner.Application.DTOs.Ingredients;

namespace MealPlanner.Application.Services;

public interface IIngredientService
{
    Task<IngredientDto?> GetIngredientByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<IngredientDto>> GetAllIngredientsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<IngredientDto>> SearchIngredientsByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IngredientDto> CreateIngredientAsync(SaveIngredientDto dto, CancellationToken cancellationToken = default);
    Task UpdateIngredientAsync(int id, SaveIngredientDto dto, CancellationToken cancellationToken = default);
    Task DeleteIngredientAsync(int id, CancellationToken cancellationToken = default);
}
