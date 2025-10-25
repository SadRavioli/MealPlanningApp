using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Repositories;
using MealPlanner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Infrastructure.Repositories;

public class ShoppingListRepository : Repository<ShoppingList>, IShoppingListRepository
{
    public ShoppingListRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ShoppingList>> GetByHouseholdIdAsync(int householdId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(sl => sl.HouseholdId == householdId)
            .OrderByDescending(sl => sl.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<ShoppingList?> GetByIdWithItemsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(sl => sl.Items)
                .ThenInclude(sli => sli.Ingredient)
            .FirstOrDefaultAsync(sl => sl.Id == id, cancellationToken);
    }
}
