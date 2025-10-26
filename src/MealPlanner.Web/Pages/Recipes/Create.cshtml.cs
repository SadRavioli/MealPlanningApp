using MealPlanner.Application.DTOs;
using MealPlanner.Application.Services;
using MealPlanner.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MealPlanner.Web.Pages.Recipes;

public class CreateModel : PageModel
{
    private readonly IRecipeService _recipeService;
    private readonly IIngredientRepository _ingredientRepository;

    public CreateModel(IRecipeService recipeService, IIngredientRepository ingredientRepository)
    {
        _recipeService = recipeService;
        _ingredientRepository = ingredientRepository;
    }

    [BindProperty]
    public SaveRecipeDto Recipe { get; set; } = new();

    public SelectList? AvailableIngredients { get; set; }

    public async Task OnGetAsync()
    {
        // Load available ingredients for dropdown
        var ingredients = await _ingredientRepository.GetAllAsync();
        AvailableIngredients = new SelectList(ingredients, "Id", "Name");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            // Reload ingredients if validation fails
            var ingredients = await _ingredientRepository.GetAllAsync();
            AvailableIngredients = new SelectList(ingredients, "Id", "Name");
            return Page();
        }

        // TODO: Get householdId from authenticated user when auth is added
        var householdId = 1;

        await _recipeService.CreateRecipeAsync(householdId, Recipe);
        return RedirectToPage("./Index");
    }
}
