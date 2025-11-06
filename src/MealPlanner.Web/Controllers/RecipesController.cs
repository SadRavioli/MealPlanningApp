using MealPlanner.Application.DTOs.Recipes;
using MealPlanner.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.Web.Controllers;

[ApiController]
[Route("api/recipes")]
public class RecipesController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    public RecipesController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    /// <summary>
    /// Get a recipe by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeDto>> GetRecipe(int id, CancellationToken cancellationToken)
    {
        var recipe = await _recipeService.GetRecipeByIdAsync(id, cancellationToken);

        if (recipe == null)
        {
            return NotFound();
        }

        return Ok(recipe);
    }

    /// <summary>
    /// Get recipes by household
    /// </summary>
    [HttpGet("household/{householdId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RecipeDto>>> GetRecipesByHousehold(int householdId, CancellationToken cancellationToken)
    {
        var recipes = await _recipeService.GetRecipesByHouseholdAsync(householdId, cancellationToken);
        return Ok(recipes);
    }

    /// <summary>
    /// Search recipes by name
    /// </summary>
    [HttpGet("household/{householdId}/search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RecipeDto>>> SearchRecipes(int householdId, [FromQuery] string searchTerm, CancellationToken cancellationToken)
    {
        var recipes = await _recipeService.SearchRecipesAsync(householdId, searchTerm, cancellationToken);
        return Ok(recipes);
    }

    /// <summary>
    /// Create a new recipe
    /// </summary>
    [HttpPost("household/{householdId}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecipeDto>> CreateRecipe(int householdId, SaveRecipeDto dto, CancellationToken cancellationToken)
    {
        var recipe = await _recipeService.CreateRecipeAsync(householdId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
    }

    /// <summary>
    /// Update an existing recipe
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRecipe(
        int id,
        SaveRecipeDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _recipeService.UpdateRecipeAsync(id, dto, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Delete an existing recipe
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRecipe(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _recipeService.DeleteRecipeAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Scale a recipe to a different number of servings
    /// </summary>
    [HttpGet("{id}/scale")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecipeDto>> ScaleRecipe(
        int id,
        [FromQuery] int servings,
        CancellationToken cancellationToken)
    {
        try
        {
            var scaledRecipe = await _recipeService.ScaleRecipeAsync(id, servings, cancellationToken);
            return Ok(scaledRecipe);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
