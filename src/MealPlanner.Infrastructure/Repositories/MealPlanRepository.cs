using MealPlanner.Domain.Entities;
using MealPlanner.Domain.Repositories;
using MealPlanner.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MealPlanner.Infrastructure.Repositories;

public class MealPlanRepository : Repository<MealPlan>, IMealPlanRepository
{
    public MealPlanRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<MealPlan>> GetByHouseholdIdAsync(int householdId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(mp => mp.HouseholdId == householdId)
            .OrderByDescending(mp => mp.WeekStartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<MealPlan?> GetByWeekStartDateAsync(int householdId, DateTime weekStartDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(mp => mp.HouseholdId == householdId && mp.WeekStartDate == weekStartDate, cancellationToken);
    }

    public async Task<MealPlan?> GetByIdWithMealsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(mp => mp.PlannedMeals)
                .ThenInclude(pm => pm.Recipe)
            .FirstOrDefaultAsync(mp => mp.Id == id, cancellationToken);
    }
}
