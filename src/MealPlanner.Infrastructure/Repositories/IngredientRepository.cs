using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Repositories;
using MealPlanner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Infrastructure.Repositories;

public class IngredientRepository : Repository<Ingredient>, IIngredientRepository
{
    public IngredientRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Ingredient?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(i => i.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<Ingredient>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.Name.ToLower().Contains(searchTerm.ToLower()))
            .OrderBy(i => i.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Ingredient>> GetByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.Category == category)
            .OrderBy(i => i.Name)
            .ToListAsync(cancellationToken);
    }
}
