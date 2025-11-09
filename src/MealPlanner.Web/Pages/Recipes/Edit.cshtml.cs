using MealPlanner.Application.DTOs.Ingredients;
using MealPlanner.Application.DTOs.Recipes;
using MealPlanner.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MealPlanner.Web.Pages.Recipes;

[Authorize]
public class EditModel : PageModel
{
    private readonly IRecipeService _recipeService;
    private readonly IIngredientService _ingredientService;

    public EditModel(IRecipeService recipeService, IIngredientService ingredientService)
    {
        _recipeService = recipeService;
        _ingredientService = ingredientService;
    }

    [BindProperty]
    public SaveRecipeDto Recipe { get; set; } = new();

    public int RecipeId { get; set; }

    public IEnumerable<IngredientDto> AvailableIngredients { get; set; } = new List<IngredientDto>();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var recipe = await _recipeService.GetRecipeByIdAsync(id);

        if (recipe == null)
        {
            return NotFound();
        }

        RecipeId = id;

        // Load available ingredients
        AvailableIngredients = await _ingredientService.GetAllIngredientsAsync();

        // Map RecipeDto to SaveRecipeDto
        Recipe = new SaveRecipeDto
        {
            Name = recipe.Name,
            Description = recipe.Description,
            Instructions = recipe.Instructions,
            PrepTimeMinutes = recipe.PrepTimeMinutes,
            CookTimeMinutes = recipe.CookTimeMinutes,
            ServingSize = recipe.ServingSize,
            Ingredients = recipe.Ingredients.Select(i => new SaveRecipeIngredientDto
            {
                IngredientId = i.IngredientId,
                Quantity = i.Quantity,
                Unit = (int)i.Unit,
                Notes = i.Notes
            }).ToList()
        };

        return Page();
    }

    public async Task<IActionResult> OnGetAddIngredientRowAsync(int index)
    {
        // Load available ingredients
        var ingredients = await _ingredientService.GetAllIngredientsAsync();

        // Return partial view for new ingredient row
        return Partial("_IngredientRow", (index, ingredients));
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            RecipeId = id;
            // Reload ingredients if validation fails
            AvailableIngredients = await _ingredientService.GetAllIngredientsAsync();
            return Page();
        }

        await _recipeService.UpdateRecipeAsync(id, Recipe);
        return RedirectToPage("./Details", new { id });
    }
}
