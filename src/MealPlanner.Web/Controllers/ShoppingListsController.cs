using MealPlanner.Application.DTOs.ShoppingLists;
using MealPlanner.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.Web.Controllers;

[ApiController]
[Route("api/shopping-lists")]
public class ShoppingListsController : ControllerBase
{
    private readonly IShoppingListService _shoppingListService;

    public ShoppingListsController(IShoppingListService shoppingListService)
    {
        _shoppingListService = shoppingListService;
    }

    /// <summary>
    /// Get a shopping list by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingListDto>> GetShoppingList(int id, CancellationToken cancellationToken)
    {
        var shoppingList = await _shoppingListService.GetShoppingListByIdAsync(id, cancellationToken);

        if (shoppingList == null)
        {
            return NotFound();
        }

        return Ok(shoppingList);
    }

    /// <summary>
    /// Get all shopping lists for a specific household
    /// </summary>
    [HttpGet("household/{householdId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ShoppingListDto>>> GetShoppingListsByHousehold(
        int householdId,
        CancellationToken cancellationToken)
    {
        var shoppingLists = await _shoppingListService.GetShoppingListsByHouseholdAsync(householdId, cancellationToken);
        return Ok(shoppingLists);
    }

    /// <summary>
    /// Create a new shopping list
    /// </summary>
    [HttpPost("household/{householdId}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ShoppingListDto>> CreateShoppingList(
        int householdId,
        SaveShoppingListDto dto,
        CancellationToken cancellationToken)
    {
        var shoppingList = await _shoppingListService.CreateShoppingListAsync(dto, householdId, cancellationToken);
        return CreatedAtAction(nameof(GetShoppingList), new { id = shoppingList.Id }, shoppingList);
    }

    /// <summary>
    /// Generate a shopping list from a meal plan
    /// </summary>
    [HttpPost("generate/meal-plan/{mealPlanId}/household/{householdId}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ShoppingListDto>> GenerateFromMealPlan(
        int mealPlanId,
        int householdId,
        CancellationToken cancellationToken)
    {
        try
        {
            var shoppingList = await _shoppingListService.GenerateFromMealPlanAsync(mealPlanId, householdId, cancellationToken);
            return CreatedAtAction(nameof(GetShoppingList), new { id = shoppingList.Id }, shoppingList);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Update an existing shopping list
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateShoppingList(
        int id,
        SaveShoppingListDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _shoppingListService.UpdateShoppingListAsync(id, dto, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Delete a shopping list
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteShoppingList(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _shoppingListService.DeleteShoppingListAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Toggle an item's checked status (check/uncheck)
    /// </summary>
    [HttpPatch("{id}/items/{itemId}/toggle")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleItemChecked(
        int id,
        int itemId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _shoppingListService.ToggleItemCheckedAsync(id, itemId, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
