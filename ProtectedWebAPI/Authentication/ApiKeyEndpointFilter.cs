namespace ProtectedWebAPI.Authentication;

public class ApiKeyEndpointFilter(IConfiguration configuration) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var extractedApiKey))
        {
            //return TypedResults.Unauthorized(); 👈 Use this if you don't have to return a message
            return new UnauthorizedHttpObjectResult("API Key is missing.");
        }

        var apiKey = configuration.GetValue<string>(AuthConstants.ApiKeySectionName)!;
        if (!apiKey.Equals(extractedApiKey))
        {
            //return TypedResults.Unauthorized(); 👈 Use this if you don't have to return a message
            return new UnauthorizedHttpObjectResult("API Key is incorrect.");
        }
        
        return await next(context);
    }
}