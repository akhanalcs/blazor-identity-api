namespace ProtectedWebAPI.Authentication;

public sealed class UnauthorizedHttpObjectResult(object body) : IResult, IStatusCodeHttpResult
{
    public int? StatusCode => StatusCodes.Status401Unauthorized;

    int? IStatusCodeHttpResult.StatusCode => StatusCode;
    
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        httpContext.Response.StatusCode = StatusCode!.Value;

        if (body is string s)
        {
            await httpContext.Response.WriteAsync(s);
            return;
        }

        await httpContext.Response.WriteAsJsonAsync(body);
    }
}