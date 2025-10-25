using MealPlanner.Domain.Entities;

namespace MealPlanner.Domain.Repositories;

public interface IIngredientRepository : IRepository<Ingredient>
{
    Task<Ingredient?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Ingredient>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<Ingredient>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default);
}
