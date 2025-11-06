using MealPlanner.Application.DTOs.Households;
using MealPlanner.Application.Services;
using MealPlanner.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MealPlanner.Web.Controllers;

[ApiController]
[Route("api/households")]
public class HouseholdsController : ControllerBase
{
    private readonly IHouseholdService _householdService;

    public HouseholdsController(IHouseholdService householdService)
    {
        _householdService = householdService;
    }

    /// <summary>
    /// Get a household by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HouseholdDto>> GetHousehold(int id, CancellationToken cancellationToken)
    {
        var household = await _householdService.GetHouseholdByIdAsync(id, cancellationToken);

        if (household == null)
        {
            return NotFound();
        }

        return Ok(household);
    }

    /// <summary>
    /// Get all households for a specific user
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<HouseholdDto>>> GetHouseholdsByUser(
        string userId,
        CancellationToken cancellationToken)
    {
        var households = await _householdService.GetHouseholdsByUserAsync(userId, cancellationToken);
        return Ok(households);
    }

    /// <summary>
    /// Create a new household
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HouseholdDto>> CreateHousehold(
        SaveHouseholdDto dto,
        CancellationToken cancellationToken)
    {
        var household = await _householdService.CreateHouseholdAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetHousehold), new { id = household.Id }, household);
    }

    /// <summary>
    /// Update an existing household
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHousehold(
        int id,
        SaveHouseholdDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _householdService.UpdateHouseholdAsync(id, dto, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Delete a household
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteHousehold(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _householdService.DeleteHouseholdAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Add a member to a household
    /// </summary>
    [HttpPost("{id}/members")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddMember(
        int id,
        HouseholdMemberDto memberDto,
        CancellationToken cancellationToken)
    {
        try
        {
            await _householdService.AddMemberToHouseholdAsync(id, memberDto, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Remove a member from a household
    /// </summary>
    [HttpDelete("{id}/members/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveMember(
        int id,
        string userId,
        CancellationToken cancellationToken)
    {
        try
        {
            await _householdService.RemoveMemberFromHouseholdAsync(id, userId, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Update a member's role in a household
    /// </summary>
    [HttpPut("{id}/members/{userId}/role")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMemberRole(
        int id,
        string userId,
        [FromBody] HouseholdRole newRole,
        CancellationToken cancellationToken)
    {
        try
        {
            await _householdService.UpdateHouseholdMemberRoleAsync(id, userId, newRole, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
