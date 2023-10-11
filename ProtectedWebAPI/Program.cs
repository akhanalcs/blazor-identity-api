using Microsoft.OpenApi.Models;
using ProtectedWebAPI.ApiKeyAuthN;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Stuff I added 👇
// Add API Key Authentication
// Reference: https://stackoverflow.com/a/75059938/8644294
builder.Services
    .AddAuthentication() // Specify default AuthN scheme here if you'd like, for eg: .AddAuthentication("ApiKeyAuth")
    .AddScheme<ApiKeyAuthNSchemeOptions, ApiKeyAuthNSchemeHandler>(
        "ApiKeyAuth",
        opts =>
            opts.ApiKey = builder.Configuration.GetValue<string>("Authentication:ApiKey")!
    );

builder.Services.AddAuthorization(options =>
{
    // Add a policy
    options.AddPolicy("APIKeyHolderPolicy", policy =>
    {
        policy.RequireRole("APIKeyHolder");
    });
});
// Stuff I added 👆

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Stuff I added 👇
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "The API Key to access the API",
        Type = SecuritySchemeType.ApiKey,
        Name = "x-api-key",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });

    var scheme = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "ApiKey" // SecurityDefinitionName from above
        },
        In = ParameterLocation.Header
    };

    var requirement = new OpenApiSecurityRequirement()
    {
        { scheme, new List<string>() }
    };
    
    options.AddSecurityRequirement(requirement);
    // Stuff I added 👆
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Stuff I added 👇
app.UseAuthorization();
// Stuff I added 👆

app.MapGet("/weather", (HttpContext context) =>
{
    Console.WriteLine($"User accessing this endpoint is: {context?.User.Identity?.Name}"); // APIUser
    Console.WriteLine($"User accessing this endpoint `IsInRole(\"APIKeyHolder\")`: {context?.User.IsInRole("APIKeyHolder")}"); // True
    
    var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

    return Enumerable.Range(1, 5)
        .Select(index => new WeatherForecast(DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]))
        .ToArray();
})
// Stuff I added 👇
.RequireAuthorization("APIKeyHolderPolicy");
//.AddEndpointFilter<ApiKeyEndpointFilter>();
// Stuff I added 👆

app.Run();

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}