# blazor-api-identity
Sign in users and call a protected API from a Blazor Server app using Identity endpoints introduced in .NET 8.
The Identity API endpoints provide APIs for authenticating with that app, and that is all. IdentityServer and OpenIddict provide something very different.

<img width="650" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/4d0361cb-b308-46d6-b307-9d05b07be1ad">

References: [Identity API endpoints](https://andrewlock.net/exploring-the-dotnet-8-preview-introducing-the-identity-api-endpoints/), [Identity API endpoints with IdentityServer](https://andrewlock.net/can-you-use-the-dotnet-8-identity-api-endpoints-with-identityserver/).

## Add a new API Project
Add a new API project: `IdentityWebAPI`.

<img width="250" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/3700efb7-7bf6-4f3a-9720-5c46e4ef4fd6">

## Install nuget packages:
````
# The main package for SQLite EF Core support
dotnet add package Microsoft.EntityFrameworkCore.SQLite --prerelease

# Contains shared build-time components for EF Core
dotnet add package Microsoft.EntityFrameworkCore.Design --prerelease

# The ASP.NET Core Identity integration for EF Core
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --prerelease
````

Install ef tool:
````
dotnet tool update --global dotnet-ef --prerelease
````
<img width="750" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/30e54bad-ea90-430f-a073-84c0d1aa81c8">

## Add `AppDbContext`
Add connection string in `appsettings.json` and register the app dbcontext in `Program.cs`
`appsettings.json`
````
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=my_test_app.db"
  }
}
````

`AppUser`:
````
public class AppUser : IdentityUser
{
    // Add customisations here later
}
````

`AppDbContext`:
````
public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}
````

`Program.cs`
````
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ...
````

## Add Identity services:
In `Program.cs`, add this service:
````
// Add Identity services:
builder.Services
    // Configures Bearer and Cookie authentication, add core Identity services such as the UserManager, SignInManager
    .AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>();
````
The `AddIdentityApiEndpoints<>` method does several things:

1. Configures Bearer and cookie authentication.
2. Adds core Identity services such as the `UserManager`.
3. Adds services required by the Identity API endpoints such as the `SignInManager`, token providers, and a no-op `IEmailSender` implementation.

## Create the database:
Create a migration and update the database.
````
dotnet ef migrations add CreateIdentityTables -o Data/Migrations
dotnet ef database update
````
<img width="700" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/9c5a9f0c-9f13-4c3e-bd4c-61a9a23cf7a4">

<img width="200" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/58206735-d384-40f4-bb96-82befaaf9f3b">

## Add authorization to API:
<img width="350" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/4f5f6cd7-b06a-4508-af63-a476064cd7e1">                   

<img width="450" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/fccd94bd-d535-4376-9e73-079dadbe9386">                    

<img width="350" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/e1dab81e-b3be-4d8d-922b-35c33525f69a">

## Take it for a test ride:
<img width="400" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/fefe5a54-d2e9-4bfd-886d-34222aae1ff4">

## Add the Identity API endpoints:
Use prefix of `"/identity"` by using `MapGroup`.

<img width="350" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/e631dbe0-6846-460c-a506-6d170faea12b">

Check out Swagger UI:

<img width="350" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/a627fa7f-f10b-4e82-90a5-8caa16ce5872">

## Test it using Rider's built-in HttpClient support:
<img width="350" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/d8de0b98-b293-45e5-b0e7-d561cc734002">

Generate Request:

<img width="350" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/2ea76b1c-37af-4619-901c-c14ad2670152">

Result:

<img width="800" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/c52baf79-1421-454e-8a6f-fd0693f2325b">

## Registering a new user:
Using `IdentityWebAPI.http` file that was created by default when I created the API.

<img width="800" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/9c04fc69-71ac-4728-8009-8face8603fdd">

## Login and retrieve tokens:
<img width="450" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/aa55a114-9961-4324-908b-5c795e027dcd">

## Call Forecast API with bearer token
See `.http` file for an example.
## Fetch new access token
See `.http` file for an example.

## Read this [excellent article](https://andrewlock.net/should-you-use-the-dotnet-8-identity-api-endpoints/).
The recommended approach when your frontend SPA is served from the same domain as your API (and so can share cookies) is to simply use cookie authentication.

If you're not hosting your app and API on the same domain, you can still use cookies for authentication with the backend for fronted (BFF) pattern. This is an increasingly common pattern (similarly recommended by the IETF) in which you use cookies to authenticate with a backend .NET app, and then that app handles retrieving and storing the access tokens. With the BFF pattern, the tokens are never sent to the browser.

### This is the recommended way of logging in users ("cookie mode"):
````
### Login (Cookie mode, persist cookies (i.e. not session cookies))
POST http://localhost:5117/account/login?cookieMode=true&persistCookies=true
Content-Type: application/json

{
  "username": "ashish@example.com",
  "password": "SuperSecret1!"
}
````
