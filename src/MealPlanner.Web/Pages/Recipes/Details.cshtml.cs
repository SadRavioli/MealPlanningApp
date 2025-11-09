using MealPlanner.Application.DTOs.Recipes;
using MealPlanner.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MealPlanner.Web.Pages.Recipes;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IRecipeService _recipeService;

    public DetailsModel(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    public RecipeDto Recipe { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var recipe = await _recipeService.GetRecipeByIdAsync(id);

        if (recipe == null)
        {
            return NotFound();
        }

        Recipe = recipe;
        return Page();
    }
}
