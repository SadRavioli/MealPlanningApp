using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Repositories;
using MealPlanner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Infrastructure.Repositories;

public class RecipeRepository : Repository<Recipe>, IRecipeRepository
{
    public RecipeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Recipe>> GetByHouseholdIdAsync(int householdId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.HouseholdId == householdId)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Recipe?> GetByIdWithIngredientsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Recipe>> SearchByNameAsync(int householdId, string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.HouseholdId == householdId && r.Name.Contains(searchTerm))
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }
}
