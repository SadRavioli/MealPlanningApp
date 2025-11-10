using MealPlanner.Application.DTOs.Recipes;
using MealPlanner.Application.Services;
using MealPlanner.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MealPlanner.Web.Pages.Recipes;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IRecipeService _recipeService;

    public IndexModel(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public List<RecipeDto> Recipes { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadRecipesAsync();

        // If this is an HTMX request, return just the recipe grid partial
        if (Request.Headers["HX-Request"].Any())
        {
            return Partial("_RecipeGrid", Recipes);
        }

        return Page();
    }

    private async Task LoadRecipesAsync()
    {
        var householdId = User.GetHouseholdId();
        if (householdId == null)
        {
            return;
        }

        var recipes = await _recipeService.GetRecipesByHouseholdAsync(householdId.Value);

        // Filter recipes if search term is provided
        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            recipes = recipes.Where(r =>
                r.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrEmpty(r.Description) && r.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        Recipes = recipes.ToList();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _recipeService.DeleteRecipeAsync(id);
        return RedirectToPage();
    }
}
