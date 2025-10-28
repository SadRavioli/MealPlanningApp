using MealPlanner.Application.DTOs.Ingredients;
using MealPlanner.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientsController : ControllerBase
{
    private readonly IIngredientService _ingredientService;

    public IngredientsController(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    /// <summary>
    /// Get an ingredient by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IngredientDto>> GetIngredient(int id, CancellationToken cancellationToken)
    {
        var ingredient = await _ingredientService.GetIngredientByIdAsync(id, cancellationToken);

        if (ingredient == null)
        {
            return NotFound();
        }

        return Ok(ingredient);
    }

    /// <summary>
    /// Get all ingredients
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<IngredientDto>>> GetAllIngredients(CancellationToken cancellationToken)
    {
        var ingredients = await _ingredientService.GetAllIngredientsAsync(cancellationToken);
        return Ok(ingredients);
    }

    /// <summary>
    /// Search ingredients by name
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<IngredientDto>>> SearchIngredients(
        [FromQuery] string searchTerm,
        CancellationToken cancellationToken)
    {
        var ingredients = await _ingredientService.SearchIngredientsByNameAsync(searchTerm, cancellationToken);
        return Ok(ingredients);
    }

    /// <summary>
    /// Create a new ingredient
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IngredientDto>> CreateIngredient(
        SaveIngredientDto dto,
        CancellationToken cancellationToken)
    {
        var ingredient = await _ingredientService.CreateIngredientAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetIngredient), new { id = ingredient.Id }, ingredient);
    }

    /// <summary>
    /// Update an existing ingredient
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateIngredient(
        int id,
        SaveIngredientDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _ingredientService.UpdateIngredientAsync(id, dto, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Delete an ingredient
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteIngredient(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _ingredientService.DeleteIngredientAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
