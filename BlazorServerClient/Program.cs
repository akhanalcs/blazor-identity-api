using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using BlazorServerClient.Components;
using BlazorServerClient.Data;
using BlazorServerClient.Identity;
using BlazorServerClient.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(); // 👈 Changed in .NET 8

builder.Services.AddCascadingAuthenticationState(); // 👈 This is nice in .NET 8. No need to wrap routes with <CascadingAuthState>
builder.Services.AddScoped<UserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();

//builder.Services.AddAntiforgery();

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

// Configure the HttpClient for the forecast service
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

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode(); // 👈 Changed in .NET 8

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();