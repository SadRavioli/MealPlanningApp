using MealPlanner.Application.DTOs.MealPlans;
using MealPlanner.Application.DTOs.Recipes;
using MealPlanner.Application.Services;
using MealPlanner.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MealPlanner.Web.Pages.MealPlans;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IMealPlanService _mealPlanService;
    private readonly IRecipeService _recipeService;

    public IndexModel(IMealPlanService mealPlanService, IRecipeService recipeService)
    {
        _mealPlanService = mealPlanService;
        _recipeService = recipeService;
    }

    public MealPlanDto? MealPlan { get; set; }
    public DateTime WeekStartDate { get; set; }
    public IEnumerable<RecipeDto> AvailableRecipes { get; set; } = new List<RecipeDto>();

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

        // Load available recipes for the household
        AvailableRecipes = await _recipeService.GetRecipesByHouseholdAsync(householdId);
    }

    public async Task<IActionResult> OnPostAddMealAsync(int recipeId, DayOfWeek dayOfWeek, MealType mealType, DateTime weekStartDate)
    {
        // TODO: Get householdId from authenticated user when auth is added
        var householdId = 1;

        // Get or create meal plan for this week
        var mealPlans = await _mealPlanService.GetMealPlansByHouseholdAsync(householdId);
        var mealPlan = mealPlans.FirstOrDefault(mp => mp.WeekStartDate.Date == weekStartDate.Date);

        // Get the recipe to determine default servings
        var recipe = await _recipeService.GetRecipeByIdAsync(recipeId);
        if (recipe == null)
        {
            return NotFound();
        }

        var newPlannedMeal = new SavePlannedMealDto
        {
            RecipeId = recipeId,
            DayOfWeek = dayOfWeek,
            MealType = mealType,
            Servings = recipe.ServingSize
        };

        if (mealPlan == null)
        {
            // Create new meal plan with this meal
            var saveMealPlanDto = new SaveMealPlanDto
            {
                WeekStartDate = weekStartDate,
                PlannedMeals = new List<SavePlannedMealDto> { newPlannedMeal }
            };

            await _mealPlanService.CreateMealPlanAsync(householdId, saveMealPlanDto);
        }
        else
        {
            // Replace existing meal in this slot (if any) or add new meal
            var plannedMeals = mealPlan.PlannedMeals
                .Where(pm => !(pm.DayOfWeek == dayOfWeek && pm.MealType == mealType)) // Remove existing meal in this slot
                .Select(pm => new SavePlannedMealDto
                {
                    RecipeId = pm.RecipeId,
                    DayOfWeek = pm.DayOfWeek,
                    MealType = pm.MealType,
                    Servings = pm.Servings,
                    Notes = pm.Notes
                }).ToList();

            plannedMeals.Add(newPlannedMeal);

            var saveMealPlanDto = new SaveMealPlanDto
            {
                WeekStartDate = weekStartDate,
                PlannedMeals = plannedMeals
            };

            await _mealPlanService.UpdateMealPlanAsync(mealPlan.Id, saveMealPlanDto);
        }

        // Return partial view for the updated meal slot
        return await GetMealSlotPartial(dayOfWeek, mealType, weekStartDate);
    }

    public async Task<IActionResult> OnPostRemoveMealAsync(DayOfWeek dayOfWeek, MealType mealType, DateTime weekStartDate)
    {
        // TODO: Get householdId from authenticated user when auth is added
        var householdId = 1;

        // Get existing meal plan
        var mealPlans = await _mealPlanService.GetMealPlansByHouseholdAsync(householdId);
        var mealPlan = mealPlans.FirstOrDefault(mp => mp.WeekStartDate.Date == weekStartDate.Date);

        if (mealPlan == null)
        {
            return NotFound();
        }

        // Remove the planned meal
        var plannedMeals = mealPlan.PlannedMeals
            .Where(pm => !(pm.DayOfWeek == dayOfWeek && pm.MealType == mealType))
            .Select(pm => new SavePlannedMealDto
            {
                RecipeId = pm.RecipeId,
                DayOfWeek = pm.DayOfWeek,
                MealType = pm.MealType,
                Servings = pm.Servings,
                Notes = pm.Notes
            }).ToList();

        var saveMealPlanDto = new SaveMealPlanDto
        {
            WeekStartDate = weekStartDate,
            PlannedMeals = plannedMeals
        };

        await _mealPlanService.UpdateMealPlanAsync(mealPlan.Id, saveMealPlanDto);

        // Return partial view for the empty meal slot
        return await GetMealSlotPartial(dayOfWeek, mealType, weekStartDate);
    }

    public async Task<IActionResult> OnGetRecipeListAsync(DayOfWeek dayOfWeek, MealType mealType, DateTime weekStartDate)
    {
        // TODO: Get householdId from authenticated user when auth is added
        var householdId = 1;

        AvailableRecipes = await _recipeService.GetRecipesByHouseholdAsync(householdId);

        return Partial("_RecipeList", (AvailableRecipes, dayOfWeek, mealType, weekStartDate));
    }

    private async Task<IActionResult> GetMealSlotPartial(DayOfWeek dayOfWeek, MealType mealType, DateTime weekStartDate)
    {
        // Reload meal plan
        var householdId = 1;
        var mealPlans = await _mealPlanService.GetMealPlansByHouseholdAsync(householdId);
        var mealPlan = mealPlans.FirstOrDefault(mp => mp.WeekStartDate.Date == weekStartDate.Date);

        var plannedMeal = mealPlan?.PlannedMeals
            .FirstOrDefault(pm => pm.DayOfWeek == dayOfWeek && pm.MealType == mealType);

        return Partial("_MealSlot", (dayOfWeek, mealType, plannedMeal, weekStartDate));
    }

    public PlannedMealDto? GetPlannedMeal(DayOfWeek day, MealType mealType)
    {
        if (MealPlan == null) return null;

        return MealPlan.PlannedMeals
            .FirstOrDefault(pm => pm.DayOfWeek == day && pm.MealType == mealType);
    }
}
