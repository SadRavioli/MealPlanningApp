using System.Security.Claims;

namespace MealPlanner.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int? GetHouseholdId(this ClaimsPrincipal principal)
    {
        var householdIdClaim = principal.FindFirst("HouseholdId");
        if (householdIdClaim != null && int.TryParse(householdIdClaim.Value, out var householdId))
        {
            return householdId;
        }
        return null;
    }
}
