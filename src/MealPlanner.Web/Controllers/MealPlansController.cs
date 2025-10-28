using MealPlanner.Application.DTOs.MealPlans;
using MealPlanner.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.Web.Controllers;

[ApiController]
[Route("api/meal-plans")]
public class MealPlansController : ControllerBase
{
    private readonly IMealPlanService _mealPlanService;

    public MealPlansController(IMealPlanService mealPlanService)
    {
        _mealPlanService = mealPlanService;
    }

    /// <summary>
    /// Get a meal plan by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MealPlanDto>> GetMealPlan(int id, CancellationToken cancellationToken)
    {
        var mealPlan = await _mealPlanService.GetMealPlanByIdAsync(id, cancellationToken);

        if (mealPlan == null)
        {
            return NotFound();
        }

        return Ok(mealPlan);
    }

    /// <summary>
    /// Get all meal plans for a specific household
    /// </summary>
    [HttpGet("household/{householdId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MealPlanDto>>> GetMealPlansByHousehold(
        int householdId,
        CancellationToken cancellationToken)
    {
        var mealPlans = await _mealPlanService.GetMealPlansByHouseholdAsync(householdId, cancellationToken);
        return Ok(mealPlans);
    }

    /// <summary>
    /// Create a new meal plan
    /// </summary>
    [HttpPost("household/{householdId}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MealPlanDto>> CreateMealPlan(
        int householdId,
        SaveMealPlanDto dto,
        CancellationToken cancellationToken)
    {
        var mealPlan = await _mealPlanService.CreateMealPlanAsync(householdId, dto, cancellationToken);
        return CreatedAtAction(nameof(GetMealPlan), new { id = mealPlan.Id }, mealPlan);
    }

    /// <summary>
    /// Update an existing meal plan
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMealPlan(
        int id,
        SaveMealPlanDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _mealPlanService.UpdateMealPlanAsync(id, dto, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Delete a meal plan
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMealPlan(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _mealPlanService.DeleteMealPlanAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
