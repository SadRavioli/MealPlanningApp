using MealPlanner.Application.DTOs.Pantries;
using MealPlanner.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.Web.Controllers;

[ApiController]
[Route("api/households/{householdId}/pantry")]
public class PantriesController : ControllerBase
{
    private readonly IPantryService _pantryService;

    public PantriesController(IPantryService pantryService)
    {
        _pantryService = pantryService;
    }

    /// <summary>
    /// Get the pantry for a specific household
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PantryDto>> GetPantryByHousehold(int householdId, CancellationToken cancellationToken)
    {
        var pantry = await _pantryService.GetPantryByHouseholdIdAsync(householdId, cancellationToken);

        if (pantry == null)
        {
            return NotFound();
        }

        return Ok(pantry);
    }

    /// <summary>
    /// Create a new pantry for a household
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PantryDto>> CreatePantry(int householdId, CancellationToken cancellationToken)
    {
        var pantry = await _pantryService.CreatePantryAsync(householdId, cancellationToken);
        return CreatedAtAction(nameof(GetPantryByHousehold), new { householdId }, pantry);
    }

    /// <summary>
    /// Add an item to the household's pantry
    /// </summary>
    [HttpPost("items")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PantryItemDto>> AddItemToPantry(
        int householdId,
        SavePantryItemDto dto,
        CancellationToken cancellationToken)
    {
        var item = await _pantryService.AddItemToPantryAsync(householdId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetPantryByHousehold), new { householdId }, item);
    }

    /// <summary>
    /// Update a pantry item
    /// </summary>
    [HttpPut("{pantryId}/items/{itemId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePantryItem(
        int householdId,
        int pantryId,
        int itemId,
        SavePantryItemDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _pantryService.UpdatePantryItemAsync(pantryId, itemId, dto, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Remove an item from the pantry
    /// </summary>
    [HttpDelete("{pantryId}/items/{itemId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveItemFromPantry(
        int householdId,
        int pantryId,
        int itemId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _pantryService.RemoveItemFromPantryAsync(pantryId, itemId, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
