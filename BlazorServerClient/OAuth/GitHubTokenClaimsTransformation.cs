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
        var userId = principal.FindFirstValue("user_id")!;
        var userFromDb = await userManager.FindByIdAsync(userId);
        
        if (userFromDb is null)
        {
            return principal;
        }
        
        var cp = principal.Clone();
        var accessToken = userFromDb.GitHubAccessToken;
        
        var identity = cp.Identities.First(i => i.AuthenticationType == IdentityConstants.ExternalScheme);
        identity.AddClaim(new Claim("github-access-token", accessToken));
        return cp;
    }
}