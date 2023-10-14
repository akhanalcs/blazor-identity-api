using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using BlazorServerClient.Components;
using BlazorServerClient.Data;
using BlazorServerClient.Identity;
using BlazorServerClient.Services;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(); // 👈 Changed in .NET 8

builder.Services.AddCascadingAuthenticationState(); // 👈 This is nice in .NET 8. No need to wrap routes with <CascadingAuthState>
builder.Services.AddScoped<UserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddMicrosoftAccount(microsoftOptions =>
    {
        microsoftOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"]!;
        microsoftOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"]!;
    })
    // OAuthHandler will run for this service when Middleware is executed.
    // Command + Click on the method to view the handler
    // When the app.UseAuthentication middleware runs, HandleRemoteAuthenticateAsync method in OAuthHandler
    // will load the authentication session from the authorization code (one time password to be ysed for exchanging with access token)
    .AddOAuth("github", githubOptions =>
    {
        githubOptions.ClientId = builder.Configuration["Authentication:GitHub:ClientId"]!;
        githubOptions.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"]!;
        githubOptions.AuthorizationEndpoint = builder.Configuration["Authentication:GitHub:AuthorizationEndpoint"]!;
        githubOptions.TokenEndpoint = builder.Configuration["Authentication:GitHub:TokenEndpoint"]!;
        githubOptions.UserInformationEndpoint = builder.Configuration["Authentication:GitHub:UserInformationEndpoint"]!;
        githubOptions.CallbackPath = builder.Configuration["Authentication:GitHub:CallbackPath"]!;

        // This will save the token in the cookie which will go to the browser
        // which won't cause an issue because cookies can't be read by just anyone.
        // It's just that you won't be able to work with the token.
        // For eg: If your cookie lasts for 90 days but AccessToken expires in 15 minutes.
        // If you put it in the db you can refresh your tokens. THis will also make the cookie small.
        // githubOptions.SaveTokens = true;
        
        githubOptions.SignInScheme = IdentityConstants.ApplicationScheme;
        
        githubOptions.ClaimActions.MapJsonKey("sub", "id");
        githubOptions.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
        
        githubOptions.Events.OnCreatingTicket = async context =>
        {
            // Save access token in the Db here
            // var db = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
            
            using var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
            using var result = await context.Backchannel.SendAsync(request);
            var user = await result.Content.ReadFromJsonAsync<JsonElement>();
            context.RunClaimActions(user);
        };
    })
    .AddIdentityCookies();

// 👇 Stuff I added
builder.Services.AddAntiforgery();
// 👆 Stuff I added

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender, NoOpEmailSender>();

// 👇 Stuff I added
builder.Services.AddSingleton<WeatherForecastService>();

// Configure the HttpClient for the forecast service (ProtectedWebAPI)
var protectedApiUrl = builder.Configuration["ProtectedWebAPI:BaseUrl"]!;
var protectedApiKey = builder.Configuration["ProtectedWebAPI:ApiKey"]!;
builder.Services.AddHttpClient<WeatherForecastService>(client =>
{
    client.BaseAddress = new Uri(protectedApiUrl);
    client.DefaultRequestHeaders.Add("X-API-KEY", protectedApiKey); // This guy will add ApiKey to the requests
});
// 👆 Stuff I added

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

// 👇 Stuff I added. These middleware are automatically added in .NET 8 after you add AuthN and AuthZ services
//app.UseAuthentication();
//app.UseAuthorization();
// 👆 Stuff I added

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode(); // 👈 Changed in .NET 8

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();