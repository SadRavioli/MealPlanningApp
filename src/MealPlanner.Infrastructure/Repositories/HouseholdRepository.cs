using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Repositories;
using MealPlanner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Infrastructure.Repositories;

public class HouseholdRepository : Repository<Household>, IHouseholdRepository
{
    public HouseholdRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Household>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserHouseholds
            .Where(uh => uh.UserId == userId)
            .Select(uh => uh.Household)
            .ToListAsync(cancellationToken);
    }

    public async Task<Household?> GetByIdWithMembersAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(h => h.UserHouseholds)
            .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }
}
