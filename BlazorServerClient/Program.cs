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
            return Task.CompletedTask;
        };
    })
    // This AUTHENTICATION handler will be triggered on the CALLBACK PATH.
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
        
        // This will save the token in the External cookie which is pretty useless in this case.
        // Because External cookie is a temp cookie, Application cookie is what's useful.
        // If you put it in the db you can refresh your tokens.
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
            var items = context.Properties.Items;
            var loginProvider = items["LoginProvider"]!; // same as context.Principal?.Identity?.AuthenticationType;
            // Same as: var accessToken = items[".Token." + "access_token"]; // TokenKeyPrefix = ".Token.", which I learned from TokenExtensions.cs. This works only if I do: githubOptions.SaveTokens = true;
            var accessToken = context.AccessToken;
            
            using var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            using var result = await context.Backchannel.SendAsync(request);
            var userFromUserInfoEndpoint = await result.Content.ReadFromJsonAsync<JsonElement>();
            
            // At this point, my context.Principal DOES NOT have claims of "sub" and "ClaimTypes.Name"
            context.RunClaimActions(userFromUserInfoEndpoint);

            // At this point, my context.Principal has claims of "sub" and "ClaimTypes.Name"
            var gitHubId = context.Principal?.FindFirstValue("sub")!; // It's also called providerKey
            
            // Some new stuff related to doing AuthZ.
            // Only useful if I was using this (OnCreatingTicket) for AuthZ where I was already authenticated with some other scheme.
            // This won't work here because I'm not signed in yet using "SomeSchemeIPreviouslySignedInWith"
            // var authResult = await context.HttpContext.AuthenticateAsync("SomeSchemeIPreviouslySignedInWith");
            // var userClaims = authResult.Principal.Claims;
            // var metaData = authResult.Properties.Items;
            
            // Store Access token of this user in the Db if you'd like:
            var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByLoginAsync(loginProvider, gitHubId);
            // Update this user here
            if (user is not null)
            {
                // If I add a column named GitHubAccessToken to my users table, I can store the token there.
                //user.GitHubAccessToken = accessToken!;
                await userManager.UpdateAsync(user);
            }

            // If you want to add some claim here to be used in ExternalLogin.razor file's _externalLoginInfo field
            // var identity = context.Principal!.Identities.First(i => i.AuthenticationType == loginProvider);
            // identity.AddClaim(new Claim("some-external-auth-stuff", "whatever"));
        };
    })
    .AddIdentityCookies(o =>
    {
        o.ApplicationCookie!.PostConfigure(options =>
        {
            // options.Events.OnRedirectToAccessDenied = ctx =>
            // {
            //     if (ctx.Request.Path.StartsWithSegments("/github"))
            //     {
            //         return ctx.HttpContext.ChallengeAsync("github");
            //     }
            //     // Otherwise do default stuff
            //     return options.Events.OnRedirectToAccessDenied(ctx);
            // };
        });
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("github-user-read", pb =>
    {
        pb.RequireAuthenticatedUser()
            .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme)
            .RequireClaim("github-access-token");
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