using MealPlanner.Domain.Entities;

namespace MealPlanner.Domain.Repositories;

public interface IRecipeRepository : IRepository<Recipe>
{
    Task<IEnumerable<Recipe>> GetByHouseholdIdAsync(int householdId, CancellationToken cancellationToken = default);
    Task<Recipe?> GetByIdWithIngredientsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Recipe>> SearchByNameAsync(int householdId, string searchTerm, CancellationToken cancellationToken = default);
}
