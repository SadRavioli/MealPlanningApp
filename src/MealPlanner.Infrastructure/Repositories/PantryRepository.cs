using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Repositories;
using MealPlanner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Infrastructure.Repositories;

public class PantryRepository : Repository<Pantry>, IPantryRepository
{
    public PantryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Pantry?> GetByHouseholdIdAsync(int householdId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.HouseholdId == householdId, cancellationToken);
    }

    public async Task<Pantry?> GetByIdWithItemsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Items)
                .ThenInclude(pi => pi.Ingredient)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}
