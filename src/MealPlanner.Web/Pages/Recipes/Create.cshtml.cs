using MealPlanner.Application.DTOs.Ingredients;
using MealPlanner.Application.DTOs.Recipes;
using MealPlanner.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MealPlanner.Web.Pages.Recipes;

public class CreateModel : PageModel
{
    private readonly IRecipeService _recipeService;
    private readonly IIngredientService _ingredientService;

    public CreateModel(IRecipeService recipeService, IIngredientService ingredientService)
    {
        _recipeService = recipeService;
        _ingredientService = ingredientService;
    }

    [BindProperty]
    public SaveRecipeDto Recipe { get; set; } = new();

    public IEnumerable<IngredientDto> AvailableIngredients { get; set; } = new List<IngredientDto>();

    public async Task OnGetAsync()
    {
        // Load available ingredients for dropdown
        AvailableIngredients = await _ingredientService.GetAllIngredientsAsync();
    }

    public async Task<IActionResult> OnGetAddIngredientRowAsync(int index)
    {
        // Load available ingredients
        var ingredients = await _ingredientService.GetAllIngredientsAsync();

        // Return partial view for new ingredient row
        return Partial("_IngredientRow", (index, ingredients));
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            // Reload ingredients if validation fails
            AvailableIngredients = await _ingredientService.GetAllIngredientsAsync();
            return Page();
        }

        // TODO: Get householdId from authenticated user when auth is added
        var householdId = 1;

        await _recipeService.CreateRecipeAsync(householdId, Recipe);
        return RedirectToPage("./Index");
    }
}
