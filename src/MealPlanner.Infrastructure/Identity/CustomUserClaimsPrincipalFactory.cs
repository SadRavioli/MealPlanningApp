using System.Security.Claims;
using MealPlanner.Domain.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace MealPlanner.Infrastructure.Identity;

public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
{
    private readonly IHouseholdRepository _householdRepository;

    public CustomUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        IOptions<IdentityOptions> optionsAccessor,
        IHouseholdRepository householdRepository)
        : base(userManager, optionsAccessor)
    {
        _householdRepository = householdRepository;
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        // Get user's household
        var households = await _householdRepository.GetByUserIdAsync(user.Id);
        var household = households.FirstOrDefault();

        if (household != null)
        {
            identity.AddClaim(new Claim("HouseholdId", household.Id.ToString()));
        }

        return identity;
    }
}
