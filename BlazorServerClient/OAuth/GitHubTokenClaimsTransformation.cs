using System.Net.Http.Headers;
using System.Security.Claims;
using BlazorServerClient.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace BlazorServerClient.OAuth;

public class GitHubTokenClaimsTransformation(UserManager<ApplicationUser> userManager) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // ClaimTypes.Name = email
        // ClaimTypes.NameIdentifier = Id
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
        
        var userFromDb = await userManager.FindByIdAsync(userId);
        
        if (userFromDb is null)
        {
            return principal;
        }
        
        var cp = principal.Clone();
        //var accessToken = userFromDb.GitHubAccessToken;
        
        // Identity.Application
        var identity = cp.Identities.First(i => i.AuthenticationType == IdentityConstants.ApplicationScheme);
        identity.AddClaim(new Claim("github-access-token", "something-i-came-up-with"));
        return cp;
    }
}