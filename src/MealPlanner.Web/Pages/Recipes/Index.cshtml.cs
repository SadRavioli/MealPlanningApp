using MealPlanner.Application.DTOs;
using MealPlanner.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MealPlanner.Web.Pages.Recipes;

public class IndexModel : PageModel
{
    private readonly IRecipeService _recipeService;

    public IndexModel(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public List<RecipeDto> Recipes { get; set; } = new();

    public async Task OnGetAsync()
    {
        // TODO: Get householdId from authenticated user when auth is added
        // For now, hardcode to 1 for testing
        var householdId = 1;

        var recipes = await _recipeService.GetRecipesByHouseholdAsync(householdId);
        Recipes = recipes.ToList();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _recipeService.DeleteRecipeAsync(id);
        return RedirectToPage();
    }
}
