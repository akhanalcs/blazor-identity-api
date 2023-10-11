using System.Security.Claims;
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

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var apiKey = Context.Request.Headers["X-API-KEY"];
        if (string.IsNullOrEmpty(apiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("X-API-KEY is missing."));
        }
        
        if (!apiKey.Equals(Options.ApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid X-API-KEY"));
        }
        
        var claims = new[] { new Claim(ClaimTypes.Name, "API USER") };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}