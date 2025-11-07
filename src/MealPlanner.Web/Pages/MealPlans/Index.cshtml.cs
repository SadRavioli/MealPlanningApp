using MealPlanner.Application.DTOs.MealPlans;
using MealPlanner.Application.Services;
using MealPlanner.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MealPlanner.Web.Pages.MealPlans;

public class IndexModel : PageModel
{
    private readonly IMealPlanService _mealPlanService;

    public IndexModel(IMealPlanService mealPlanService)
    {
        _mealPlanService = mealPlanService;
    }

    public MealPlanDto? MealPlan { get; set; }
    public DateTime WeekStartDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? SelectedDate { get; set; }

    public async Task OnGetAsync()
    {
        // Determine which week to show
        var targetDate = SelectedDate ?? DateTime.Today;

        // Calculate the start of the week (Sunday)
        WeekStartDate = targetDate.AddDays(-(int)targetDate.DayOfWeek);

        // TODO: Get householdId from authenticated user when auth is added
        var householdId = 1;

        // Try to get existing meal plan for this week
        var mealPlans = await _mealPlanService.GetMealPlansByHouseholdAsync(householdId);
        MealPlan = mealPlans.FirstOrDefault(mp => mp.WeekStartDate.Date == WeekStartDate.Date);
    }

    public PlannedMealDto? GetPlannedMeal(DayOfWeek day, MealType mealType)
    {
        if (MealPlan == null) return null;

        return MealPlan.PlannedMeals
            .FirstOrDefault(pm => pm.DayOfWeek == day && pm.MealType == mealType);
    }
}
