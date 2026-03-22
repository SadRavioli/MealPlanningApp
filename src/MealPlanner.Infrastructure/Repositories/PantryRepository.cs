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

    public async Task<Pantry?> GetByHouseholdIdWithItemsAsync(int householdId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Items)
                .ThenInclude(pi => pi.Ingredient)
            .FirstOrDefaultAsync(p => p.HouseholdId == householdId, cancellationToken);
    }

    public async Task<PantryItem?> GetPantryItemByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.PantryItems
            .Include(pi => pi.Ingredient)
            .FirstOrDefaultAsync(pi => pi.Id == id, cancellationToken);
    }

    public async Task RemovePantryItemAsync(PantryItem entity, CancellationToken cancellationToken = default)
    {
        _context.PantryItems.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
