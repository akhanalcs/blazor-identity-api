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
using BlazorServerClient.OAuth;
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
        microsoftOptions.SignInScheme = IdentityConstants.ExternalScheme; // Very Important!

        microsoftOptions.Events.OnCreatingTicket += context =>
        {
            // This principal will be handed over to microsoftOptions.SignInScheme AuthN handler
            //context.Principal
            //return Task.CompletedTask;
        };
    })
    .AddOAuth("github", githubOptions =>
    {
        githubOptions.ClientId = builder.Configuration["Authentication:GitHub:ClientId"]!;
        githubOptions.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"]!;
        githubOptions.AuthorizationEndpoint = builder.Configuration["Authentication:GitHub:AuthorizationEndpoint"]!;
        githubOptions.TokenEndpoint = builder.Configuration["Authentication:GitHub:TokenEndpoint"]!;
        githubOptions.UserInformationEndpoint = builder.Configuration["Authentication:GitHub:UserInformationEndpoint"]!;
        
        // This can be anything. Put something unique.
        // For eg: "/signin-github" or "/signin-github-cb"
        // Just make sure to put the same in App registration as well
        githubOptions.CallbackPath = builder.Configuration["Authentication:GitHub:CallbackPath"]!; 

        // https://docs.github.com/en/apps/oauth-apps/building-oauth-apps/scopes-for-oauth-apps
        githubOptions.Scope.Add("read:user");
        
        // This will save the token in the cookie which will go to the browser
        // which won't cause an issue security wise because cookies can't be read by just anyone.
        // It's just that you won't be able to work with the token.
        // For eg: If your cookie lasts for 90 days but AccessToken expires in 15 minutes.
        // If you put it in the db you can refresh your tokens. This will also make the cookie small.
        // githubOptions.SaveTokens = true;
        
        githubOptions.SignInScheme = IdentityConstants.ExternalScheme; // Very Important!
        githubOptions.ClaimActions.MapJsonKey("sub", "id");
        githubOptions.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
        
        // Invoked after the provider successfully authenticates a user.
        // Base implementation for this is in: OAuthHandler.CreateTicketAsync
        // You can see how Microsoft does it in their package by going here: MicrosoftAccountHandler.CreateTicketAsync
        // External Cookie is not set when this runs. It'll be set after it's completed in 'HandleRequestAsync' method of 'RemoteAuthenticationHandler.cs'
        // CookieAuthenticationHandler.cs -> HandleSignInAsync
        // We come here after exchanging code for a token and before creating a ticket
        githubOptions.Events.OnCreatingTicket = async context =>
        {
            // Some stuff related to AuthN
            using var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);
            using var result = await context.Backchannel.SendAsync(request);
            var userFromUserInfoEndpoint = await result.Content.ReadFromJsonAsync<JsonElement>();
            context.RunClaimActions(userFromUserInfoEndpoint);

            var gitHubId = context.Principal?.FindFirstValue("sub");
            
            // Some new stuff related to AuthZ
            /*var authHandlerProvider = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
            var extAuthHandler = await authHandlerProvider.GetHandlerAsync(context.HttpContext, IdentityConstants.ExternalScheme);
            var appAuthHandler = await authHandlerProvider.GetHandlerAsync(context.HttpContext, IdentityConstants.ApplicationScheme);

            var extAuthResult = await extAuthHandler!.AuthenticateAsync();
            var appAuthResult = await appAuthHandler!.AuthenticateAsync();

            if (extAuthResult.Succeeded)
            {
                context.Fail("Authentication failed bruh!");
                return;
            }

            var cp = extAuthResult.Principal;
            var userId = cp?.FindFirstValue("user_id")!;

            // Store Access token on this userId
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByIdAsync(userId);
            // Update your user here
            if (user is not null)
            {
                user.GitHubAccessToken = context.AccessToken!;
                await userManager.UpdateAsync(user);
            }

            context.Principal = cp?.Clone();
            var identity = context.Principal!.Identities.First(i => i.AuthenticationType == IdentityConstants.ExternalScheme);
            identity.AddClaim(new Claim("some-custom-claim", "present"));*/
        };
    })
    .AddIdentityCookies(o =>
    {
        o.ApplicationCookie!.PostConfigure(options =>
        {
            options.Events.OnRedirectToAccessDenied = ctx =>
            {
                if (ctx.Request.Path.StartsWithSegments("/github"))
                {
                    return ctx.HttpContext.ChallengeAsync("github");
                }
                else
                {
                    // Otherwise do default stuff
                    return options.Events.OnRedirectToAccessDenied(ctx);
                }
            };
        });
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("github-user-read", pb =>
    {
        pb.RequireAuthenticatedUser()
            .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme)
            .RequireClaim("some-custom-claim", "present");
    });
});

// 👇 Stuff I added
builder.Services.AddTransient<IClaimsTransformation, GitHubTokenClaimsTransformation>();
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