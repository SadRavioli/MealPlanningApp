using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MealPlanner.Web.Extensions;
using MealPlanner.Application.Services;
using MealPlanner.Application.DTOs.Pantries;
using MealPlanner.Application.DTOs.Ingredients;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MealPlanner.Web.Pages.Food;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IPantryService _pantryService;
    private readonly IIngredientService _ingredientService;

    public IndexModel(IPantryService pantryService, IIngredientService ingredientService)
    {
        _pantryService = pantryService;
        _ingredientService = ingredientService;
    }

    public PantryDto? CurrentPantry { get; set; }
    public IEnumerable<IngredientDto> AvailableIngredients { get; set; } = [];

    [BindProperty]
    public SavePantryItemDto NewItem { get; set; } = new();

    public async Task OnGetAsync()
    {
        var householdId = User.GetHouseholdId();
        if (householdId == null) return;

        CurrentPantry = await _pantryService.GetPantryByHouseholdIdAsync(householdId.Value);
        AvailableIngredients = await _ingredientService.GetAllIngredientsAsync();
    }

    // Returns the add form partial for HTMX to swap in
    public async Task<IActionResult> OnGetAddFormAsync()
    {
        AvailableIngredients = await _ingredientService.GetAllIngredientsAsync();
        return Partial("_PantryAddForm", this);
    }

    // Handles form submission, returns the new item partial for HTMX to swap in
    public async Task<IActionResult> OnPostAddItemAsync()
    {
        var householdId = User.GetHouseholdId();
        if (householdId == null) return BadRequest();

        if (NewItem.ExpiryDate.HasValue)
            NewItem.ExpiryDate = DateTime.SpecifyKind(NewItem.ExpiryDate.Value, DateTimeKind.Utc);

        var item = await _pantryService.AddItemToPantryAsync(householdId.Value, NewItem);

        if (item.WasExisting)
        {
            Response.Headers["HX-Retarget"] = $"#pantry-item-{item.Id}";
            Response.Headers["HX-Reswap"] = "outerHTML";
        }

        return Partial("_PantryListItem", item);
    }

    public async Task<IActionResult> OnPostRemoveItemAsync(int itemId)
    {
        var householdId = User.GetHouseholdId();
        if (householdId == null) return BadRequest();

        await _pantryService.RemoveItemFromPantryAsync(itemId);

        return new EmptyResult();
    }
}
