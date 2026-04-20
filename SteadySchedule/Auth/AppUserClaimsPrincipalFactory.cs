using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SteadySchedule.Data;

public class AppUserClaimsPrincipalFactory
    : UserClaimsPrincipalFactory<ApplicationUser>
{
    public AppUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        identity.AddClaim(new Claim("CompanyId", user.CompanyId.ToString()));

        return identity;
    }
}