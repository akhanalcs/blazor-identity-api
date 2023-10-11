# blazor-identity-api
Sign in users using `AspNetCore.Identity` (`.NET 8 RC2`) in a Blazor Server app using cookie authentication and call a protected API using API Key authentication.

Run both of the projects, login using Username: `ashish@example.com` and Password: `Password123!`.
Navigate to Weather page and you can see the weather data being fetched from a secured API:

<img width="870" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/84982d65-7bfa-4d33-b749-3fcb03b3688f">

## How I created the projects
### BlazorServerClient project
1. Installed ef tool
   `dotnet tool update --global dotnet-ef --prerelease`
   
2. I used Rider to create the project:
   <img width="770" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/39a2da4e-bee7-40ec-9ff6-7d077c0d057d">

   Hit Create

3. I ran the migrations
   `dotnet ef database update`

4. Added some missing middleware not included in the template
   
   <img width="220" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/060dd0aa-1b65-4b6d-96cd-6d8b708f1ca7">

5. Launched the app, created a new user and signed right in.

### ProtectedWebAPI project
1. I used Rider to create the project
   
   <img width="550" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/88f370b8-f6e8-43c0-849e-be30d5d0cf30">
   
3. Added API Key authentication to it. Take a look at the code to see how I implemented it. I referenced mostly [this](https://github.com/jpdillingham/HMACAuth) and [this](https://stackoverflow.com/questions/70277577/asp-net-core-simple-api-key-authentication/75059938#75059938).
   
## Issues in the BlazorServerClient project
1. The Logout doesn't work:
   
   Click `Logout` on the bottom left.
   
   <img width="650" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/74d888f4-4b26-4b81-8134-fe9cdef72ffb">

   You'll get this error and the user will be never logged out.
   <img width="1675" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/7bf641f1-7542-4f11-87cb-7a69a594b3f4">



3. Lot of errors show up. Could be a bug in Rider. (The app runs fine though).
   
   <img width="1405" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/bda6aae4-1bf8-4e3a-b31b-d05dba1038f0">


## Call ProtectedWebAPI from BlazorServerClient
In the client project, I setup the ProtectedWebAPI Url and ApiKey in appsettings.json and used that info in Program.cs to call the API.

appsettings.json:

<img width="350" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/f8a99aa4-6768-447b-bc4c-e9752d1e896b">

Program.cs

<img width="650" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/81f7e84a-4360-45a1-90d7-360b28c003b6">

WeatherForecastService.cs:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-api-identity/assets/30603497/b518a68a-a07f-417b-a9fb-dac19ea7e94f">
