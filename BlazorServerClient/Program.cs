using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using BlazorServerClient.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

// Configure the HttpClient for the forecast service
var protectedApiUrl = builder.Configuration["ProtectedWebAPI:BaseUrl"]!;
// I went with IDownstreamApi _downstreamApi, so don't really need this to call the backend API
builder.Services.AddHttpClient<WeatherForecastService>(client =>
{
    client.BaseAddress = new Uri(protectedApiUrl);
});
// Extra stuff I added - End

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();