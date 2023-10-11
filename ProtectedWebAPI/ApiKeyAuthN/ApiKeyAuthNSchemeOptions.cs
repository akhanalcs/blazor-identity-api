using Microsoft.AspNetCore.Authentication;

namespace ProtectedWebAPI.ApiKeyAuthN;

public class ApiKeyAuthNSchemeOptions : AuthenticationSchemeOptions
{
    public string ApiKey { get; set; } = null!;
}