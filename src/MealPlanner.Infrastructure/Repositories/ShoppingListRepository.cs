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

    public async Task<IReadOnlyList<ShoppingList>> GetByHouseholdIdAsync(int householdId, CancellationToken cancellationToken = default)
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
    
    public async Task<ShoppingList?> GetActiveListByHouseholdAsync(int householdId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(sl => sl.Items)
                .ThenInclude(sli => sli.Ingredient)
            .FirstOrDefaultAsync(sl => sl.HouseholdId == householdId, cancellationToken);
    }

    public async Task<ShoppingListItem?> GetListItemByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.ShoppingListItems
            .Include(sli => sli.Ingredient)
            .FirstOrDefaultAsync(sli => sli.Id == id, cancellationToken);
    }

    public async Task UpdateItemAsync(ShoppingListItem item, CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
