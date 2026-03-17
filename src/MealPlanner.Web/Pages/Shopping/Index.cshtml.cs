using MealPlanner.Application.DTOs.Ingredients;
using MealPlanner.Application.DTOs.MealPlans;
using MealPlanner.Application.DTOs.ShoppingLists;
using MealPlanner.Application.Services;
using MealPlanner.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MealPlanner.Web.Pages.Shopping;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IShoppingListService _shoppingListService;
    private readonly IMealPlanService _mealPlanService;
    private readonly IIngredientService _ingredientService;

    public IndexModel(
        IShoppingListService shoppingListService, 
        IMealPlanService mealPlanService,
        IIngredientService ingredientService)
    {
        _shoppingListService = shoppingListService;
        _mealPlanService = mealPlanService;
        _ingredientService = ingredientService;
    }

    public ShoppingListDto? CurrentShoppingList { get; set; }
    public IEnumerable<MealPlanDto> RecentMealPlans { get; set; } = new List<MealPlanDto>();
    public IEnumerable<IngredientDto> AvailableIngredients { get; set; } = new List<IngredientDto>();

    [BindProperty]
    public SaveShoppingListItemDto NewItem { get; set; } = new();

    public async Task OnGetAsync()
    {
        var householdId = User.GetHouseholdId();
        if (householdId == null) return;

        var lists = await _shoppingListService.GetShoppingListsByHouseholdAsync(householdId.Value);
        var latestList = lists.OrderByDescending(l => l.Id).FirstOrDefault();
        
        if (latestList != null)
        {
            CurrentShoppingList = await _shoppingListService.GetShoppingListByIdAsync(latestList.Id);
        }

        var mealPlans = await _mealPlanService.GetMealPlansByHouseholdAsync(householdId.Value);
        RecentMealPlans = mealPlans.OrderByDescending(mp => mp.WeekStartDate).Take(3);

        AvailableIngredients = await _ingredientService.GetAllIngredientsAsync();
    }

    public async Task<IActionResult> OnPostGenerateAsync(int mealPlanId)
    {
        var householdId = User.GetHouseholdId();
        if (householdId == null) return Unauthorized();

        await _shoppingListService.GenerateFromMealPlanAsync(mealPlanId, householdId.Value);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostToggleItemAsync(int listId, int itemId)
    {
        var householdId = User.GetHouseholdId();
        if (householdId == null) return Unauthorized();

        await _shoppingListService.ToggleItemCheckedAsync(listId, itemId);

        if (Request.Headers["HX-Request"].Count > 0)
        {
            var item = await _shoppingListService.GetShoppingListItemByIdAsync(itemId);
            if (item == null) return NotFound();

            ViewData["listId"] = listId; // Pass listId to the partial for the hx-post URL            
            return Partial("_ShoppingListItem", item);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAddItemAsync(int listId)
    {
        var householdId = User.GetHouseholdId();
        if (householdId == null) return Unauthorized();

        var list = await _shoppingListService.GetShoppingListByIdAsync(listId);
        if (list == null) return NotFound();

        var saveDto = new SaveShoppingListDto
        {
            MealPlanId = list.MealPlanId,
            Notes = list.Notes,
            Items = list.Items.Select(i => new SaveShoppingListItemDto
            {
                IngredientId = i.IngredientId,
                Quantity = i.Quantity,
                Unit = i.Unit
            }).Append(NewItem).ToList()
        };

        await _shoppingListService.UpdateShoppingListAsync(listId, saveDto);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var householdId = User.GetHouseholdId();
        if (householdId == null) return Unauthorized();

        await _shoppingListService.DeleteShoppingListAsync(id);
        return RedirectToPage();
    }
}
