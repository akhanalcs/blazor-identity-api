using System.Security.Principal;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace ProtectedWebAPI.ApiKeyAuthN;

public class ApiKeyAuthNSchemeHandler : AuthenticationHandler<ApiKeyAuthNSchemeOptions>
{
    public ApiKeyAuthNSchemeHandler(
        IOptionsMonitor<ApiKeyAuthNSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var apiKey = Context.Request.Headers["X-API-KEY"];
        if (string.IsNullOrEmpty(apiKey))
        {
            return AuthenticateResult.Fail("X-API-KEY is missing.");
        }
        
        if (!apiKey.Equals(Options.ApiKey))
        {
            return AuthenticateResult.Fail("Invalid X-API-KEY");
        }
        
        // var claims = new[] { new Claim(ClaimTypes.Name, "APIUser") };
        // var identity = new ClaimsIdentity(claims, Scheme.Name); // Scheme.Name = "ApiKeyAuth" set in Program.cs
        // var principal = new ClaimsPrincipal(identity);
        // var ticket = new AuthenticationTicket(principal, Scheme.Name);
        // return AuthenticateResult.Success(ticket);
        
        return AuthenticateResult.Success(new AuthenticationTicket(
            new GenericPrincipal(new GenericIdentity("APIUser"), new[] { "ApiKeyHolder" }),
            new AuthenticationProperties() { IsPersistent = false, AllowRefresh = false },
            Scheme.Name));
    }
}