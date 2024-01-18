# blazor-identity-api
Sign in users using `AspNetCore.Identity` (`.NET 8 RC2`) in a Blazor Server app using cookie authentication and call a protected API using API Key authentication.

Run both of the projects, login using Username: `ashish@example.com` and Password: `Password123!`.
Navigate to Weather page and you can see the weather data being fetched from a secured API:

<img width="870" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/84982d65-7bfa-4d33-b749-3fcb03b3688f">

## Helpful links
1. [Choosing an Identity Solution: Identity vs OIDC](https://learn.microsoft.com/en-us/aspnet/core/security/how-to-choose-identity-solution?view=aspnetcore-8.0#identity-vs-oidc-server) (Read it!)
2. [Blazor OIDC with Aspire Example](https://github.com/dotnet/blazor-samples/tree/main/8.0/BlazorWebAppOidc)
3. [ASP.NET Core Identity system is designed to authenticate a single web application](https://github.com/dotnet/aspnetcore/issues/52800#issuecomment-1856750779)

## How I created the projects
### BlazorServerClient project
1. Installed ef tool
   `dotnet tool update --global dotnet-ef --prerelease`
   
2. I used Rider to create the project:
   <img width="770" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/39a2da4e-bee7-40ec-9ff6-7d077c0d057d">

   Hit Create

3. I ran the migrations
   `dotnet ef database update`

4. Added some missing middleware not included in the template
   
   <img width="220" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/060dd0aa-1b65-4b6d-96cd-6d8b708f1ca7">

5. Launched the app, created a new user and signed right in.

### ProtectedWebAPI project
1. I used Rider to create the project
   
   <img width="550" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/88f370b8-f6e8-43c0-849e-be30d5d0cf30">
   
3. Added API Key authentication to it. Take a look at the code to see how I implemented it. I referenced mostly [this](https://github.com/jpdillingham/HMACAuth) and [this](https://stackoverflow.com/questions/70277577/asp-net-core-simple-api-key-authentication/75059938#75059938).
   
## Issues in the BlazorServerClient project
1. The Logout doesn't work:
   
   Click `Logout` on the bottom left.
   
   <img width="650" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/74d888f4-4b26-4b81-8134-fe9cdef72ffb">

   You'll get this error and the user will be never logged out.
   <img width="1675" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/7bf641f1-7542-4f11-87cb-7a69a594b3f4">



3. Lot of errors show up. Could be a bug in Rider. (The app runs fine though).
   
   <img width="1405" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/bda6aae4-1bf8-4e3a-b31b-d05dba1038f0">


## Call ProtectedWebAPI from BlazorServerClient
In the client project, I setup the ProtectedWebAPI Url and ApiKey in appsettings.json and used that info in Program.cs to call the API.

appsettings.json:

<img width="350" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/f8a99aa4-6768-447b-bc4c-e9752d1e896b">

Program.cs

<img width="650" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/81f7e84a-4360-45a1-90d7-360b28c003b6">

WeatherForecastService.cs:

<img width="750" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/b518a68a-a07f-417b-a9fb-dac19ea7e94f">

## Add Microsoft Authentication to your app. [Reference](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/microsoft-logins?view=aspnetcore-8.0).

### Create an app in Microsoft Developer Portal (Azure)
<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/ecb5b07c-63d1-4dd0-9eff-2bf0faa4eb42">

### Store your config
Store the secret in `user-secrets`. Store ClientId in appsettings.json.

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/7fe8b6ed-1e3e-4ee6-96a1-b3a1a343b1b2">

### Add Nuget package
<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/076100a7-876b-43aa-9757-09deac4c11ed">

### Setup `Program.cs`
<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/930f4184-26f4-4fdb-a6e1-ca7e84790eb8">

## Take OAuth authentication for a test drive

1. Click "Microsoft"
   
   <img width="750" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/4e9d7a36-78c9-4259-a708-0048e16207c3">

2. Give consent
   
   <img width="450" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/d68ae718-d4b9-4925-bf6a-51c3ac12df26">

3. These are the claims, MSFT sends to the app

   <img width="550" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/77512a98-c156-47de-a7c1-5fe27adb2e03">

## How the claims showed up in the UI
The services are setup at the last line of `MicrosoftAccountExtensions` where there's a call to `.AddOAuth`.
Here you can see the `MicrosoftAccountHandler`.

<img width="950" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/f93e19a0-e5b7-4380-8497-9a75b7e7c088">

To see how the above claims were fetched, you can see it in the `MicrosoftAccountOptions` class added from the package. Here you can see that it had asked for the scope of `user.read` and Claims were mapped this way:

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/82aa93ac-f81f-4c36-988e-9c9228c2512c">

## How Microsoft Authentication is setup under the covers
Command + Click on `.AddMicrosoftAccount` method:

<img width="650" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/ef6ff0ff-9b6b-43e0-a39d-28ea90500e8f">

Check what `AuthenticationScheme` was used:

<img width="950" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/674d6247-cb1c-4e43-9816-e4d125b7a792">

By going to `MicrosoftAccountDefaults`:

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/fdbe90fb-286c-46c1-8d2b-0e07f4015a49">

You can see `AuthenticationScheme` used was `"Microsoft"` and also see the 3 most important endpoints in OAuth: `AuthorizationEndpoint`, `TokenEndpoint` and `UserInformationEndpoint`.

## Add GitHub authentication to your app. [Reference](https://youtu.be/PUXpfr1LzPE?si=LTlb0vyOqLXPiQ4t).

### Register an app in Github
<img width="550" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/b0a2fc52-b8a5-4e63-8b20-a18944848b64">

Grab `clientid` and `clientsecret`.

### Go to GitHub's OAuth docs to find 3 important endpoints as part of OAuth: Authorize, Token and User endpoints.[Reference](https://docs.github.com/en/apps/oauth-apps/building-oauth-apps/authorizing-oauth-apps).

1. Authorize
   
   <img width="350" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/77d4ecd3-d5aa-4a42-829f-46f6498fbeec">
   
   After the user is logged in, GitHub sends us the one time code (that can be used to exchange for a token) to the redirect url that we set during registration.
   
   `https://localhost:7074/signin-github`
   
3. Token
   
   <img width="650" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/4f740b74-57b0-47bc-89a0-e71e34de849b">

4. User Information endpoint to get user info
   
   <img width="450" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/ddc26219-7642-4501-878b-625d3620af5c">

### Store your config
<img width="550" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/4dbaf4b6-b43b-4412-9be3-bd09e6dd3089">

### Setup `Program.cs`
Just look at the code.

## AuthN and AuthZ Basics [Reference](https://youtu.be/02Yh3sxzAYI?si=LAFGO54TlB7am5Gn).
### Big picture
`app.UseRouting()`: URL is matched to the endpoint.

`app.UseEndpoints()`: Actual endpoints are registered.

<img width="800" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/a605b6d9-173e-41e6-b0c2-1fa166347ebc">

### Cookie
This is what cookie contains

<img width="700" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/63622179-0729-4005-94b2-bb182dcc9c6d">

You can retrieve those `Items` from `AuthenticateResult`.`AuthenticationProperties`.`Items`:

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/6fba6417-9d36-46c3-aca6-1e37ada61703">

### External Authentication
It is handled by `RemoteAuthenticationHandler`.

<img width="700" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/9560406a-76b6-4b84-8631-3cdf9938810c">

#### Propietary provider support:
Google, Facebook, Twitter, Microsoft Account etc

#### Standard Protocol support:
OpenID connect, WS-Federation, SAML etc.

## Taking a look at Authentication middleware
Every time you navigate to ANY page in the app, the `Authentication` middleware runs (**It's middleware duh!**).

It's the bit that's inside `app.UseAuthentication`:

<img width="650" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/8c89eac8-2b5f-4ae0-8840-94da5bd0e3bd">

### Handlers:
See how `IAuthenticationHandler` looks like:

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/01b3b931-2628-4f28-a25a-3143f2f34503">

See how `IAuthenticationRequestHandler` looks like (this is relevant in `var handler = await handlers.GetHandlerAsync(context, scheme.Name) as IAuthenticationRequestHandler;` line shown below):

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/958e94de-0abc-4d23-90e8-4272edde5a6a">

I have setup Microsoft and Github (OAuth) authentication, so I've got 2 handlers now:

<img width="650" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/99bbc7d8-27c2-4c5f-87cd-72c7fe71ef84">

Those 2 handlers come from the service registration section:

<img width="250" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/dac16155-7c29-413c-8f5b-9dac327dc2de">

The middleware determines if it should handle auth request on those handlers (using `IAuthenticationRequestHandler.HandleRequestAsync`). 

<img width="650" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/6daaf501-1804-4f26-807c-5eee61705fe1">

For eg: For my case (Microsoft, GitHub handlers), it's determined by `HandleRequestAsync()` in `RemoteAuthenticationHandler.cs`:

<img width="450" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/c2844e22-93ec-474c-9b5e-fed5a897d81b">

Since, I'm going to "/counter" page now, the `HandleRequestAsync` method short circuits.

### DefaultAuthentication:
Default AuthenticationScheme is whatever I setup in `.AddAuthentication`:

<img width="650" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/6019d4bb-c0a0-430b-8bcf-bc7bee007bbd">

Using the default authentication scheme, it tries to authenticate the current request. If it succeeds, you get `.User` in the HttpContext.

<img width="650" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/e018d9a7-88cd-4443-bd18-b77d69da11bd">

To view the handler for your default auth scheme, navigate from here:

<img width="650" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/c4d0281d-ee29-4886-90ef-f923537385f6">

To here:

<img width="650" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/a358222d-cc17-4d80-9a1f-016f35cc496e">

To here:

<img width="650" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/d94e3e07-8435-4c39-83e9-70ef66b7756b">

To here:

<img width="650" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/3e122c7a-0322-4b8b-8bd0-042a56f09b2b">

To finally here:

<img width="750" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/ec95dcf2-9209-4801-a89d-2aab2f50428c">

Here you can see that this service has Schemes, Handlers etc. to authenticate a request.

Now let's get back to see how this line in `AuthenticationMiddleware.cs` executes:

<img width="650" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/230d9e13-1e78-4892-9ff3-8877a10b574b">

It just calls `.AuthenticateAsync` on the `AuthenticationService`:

<img width="650" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/c11a1bbe-131d-4777-98b3-9910f34cdfba">

#### Let's see this in action:
See the SchemeName and Handler:

<img width="750" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/d7220f34-d95e-437b-8fe6-ed7c2be51555">

Now we get into `AuthenticateService`'s `AuthenticateAsync` method:

<img width="750" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/5b4e3109-3aac-4030-b583-129f18ab6295">

Then into `AuthenticationHandler`'s `AuthenticateAsync` method:

<img width="750" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/360f549f-e8a6-400e-aebc-346496e29b9f">

Then into `AuthenticationHandler`'s `HandleAuthenticateOnceAsync` method:

<img width="450" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/2d5fb0e8-a0ff-4826-b38a-0d44b87f0f67">

Then into `AuthenticationHandler`'s `HandleAuthenticateAsync` method:

<img width="450" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/00c8d5d7-062c-4266-8fb9-4d66c5c5e52d">

It's an abstract method that is implemented in a class that derives it, `CookieAuthenticationHandler` in this case, so we end up here:

<img width="750" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/b91dede3-889a-431c-b586-1de877bb1ad8">

Then we finally get this result:

<img width="750" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/cf863f30-b60b-4605-809f-c653cab8b4da">

## Taking a look at GitHub Authentication in detail
The schemes the app has:

<img width="550" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/36b17dc7-1a95-4cbe-98a7-b5bfd6e140bf">

`ExternalLoginPicker.razor` shows the external logins:

<img width="900" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/c6b90469-acf2-4317-b0d1-1e44ca7ce5ab">

### Click "OAuth" button.

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/917e57cb-8608-433d-b6d0-3de029224c29">

This will call the POST endpoint:

<img width="700" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/9b59d135-20b7-4400-bce6-8f917add9f66">

`/Account/PerformExternalLogin` in `Identity/Extensions/IdentityComponentsEndpointRouteBuilderExtensions.cs`

This is where I want GitHub to redirect me after completing authentication:

<img width="650" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/183c5061-7d9e-4d91-923c-f2ac911bb891">

Use `properties` to preserve data between Challenge phase and Callback phase

<img width="700" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/1d5b1e07-eecd-47b2-84b8-7b2687c7f4f8">

### Challenge (scheme)
<img width="450" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/6636b1d2-e371-4546-98b5-560ece855a0b">

Now we're in `OAuthHandler.cs`

<img width="800" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/e614aba9-b0ea-41d3-8414-216683697b1a">

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/cb1144bd-c697-40fb-951d-f6d7c050d3fd">

### Redirect to external provider
`https://localhost:7074/Account/PerformExternalLogin` redirects us to GitHub's authorization endpoint:

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/50538aa5-0641-434a-ae10-2e206cb08481">

So the app goes to that location:

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/6c1de721-730c-4a9b-9212-f1c0d37c5f34">

At this ppint the user authenicates with GitHub (NOT this app) and the user authorizes this app to fetch user info from GitHub by accepting the consent screen.

### Callback
User gets redirected with the one time code to the callback url:

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/b00ae5cb-8049-4f04-a66a-7028dc05c30e">

#### AuthN middleware asks handlers who wants to process request
<img width="800" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/bd2a2f49-ec27-484c-ac78-4c8d2fc1f1fb">

`OAuthHandler` says "I will" because `ShouldHandleRequestAsync()` returns `true` as seen in `RemoteAuthenticationHandler.cs`:

<img width="800" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/418de550-8f9a-489b-b466-5c6e6d065c72">

#### Handler does the protocol post-processing
When line 87 shown above runs, we end up in `OAuthHandler.cs`:

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/f8c82466-55ac-4b97-a8ba-533df1165d57">

The query has code and state.

<img width="750" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/e5d61a96-0f32-492a-a095-68ce34382dac">

The state has redirect url and login provider we set earlier:

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/4852f83e-af86-4983-97be-f79352ef52c0">

The code is exchanged for the token here:

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/51dc3939-bbd1-42e9-9c4f-14b5c0cff9ac">

Dummy identity is created:

<img width="800" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/3aa27088-72af-4dfd-9549-8c84b5115e72">

Towards the end of this method, a ticket is created:

<img width="950" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/9482726e-92c9-497f-ae8f-f3821a08e2c6">

By calling this callback:

<img width="750" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/d5a31a96-1eee-42b3-a171-cff98e00b9eb">

Now we're back in `RemoteAuthenticationHandler.cs`.

Inside `HandleRequestAsync()`, we set which scheme produced this identity:

<img width="900" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/ded03b88-3c8a-4ab2-bd51-268087cdfa26">

#### Call sign-in handler (Set "External" cookie)
In `RemoteAuthenticationHandler.cs`

<img width="800" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/a7b6a17e-93c6-437e-8172-3cc725d8d117">

The `Principal` looks like this:

<img width="800" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/efdedcb0-6096-4fa7-a64b-1f2d06639284">

And the properties:

<img width="800" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/a7bd09e9-05fb-45d0-98e4-103c85f26f2e">

Now we're in `CookieAuthenticationHandler.cs`'s `HandleSignInAsync` method:

<img width="800" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/53828fd4-b44a-41f8-93fd-8adba174305b">

A new ticket is created:

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/62d52eda-c8be-4219-984a-b71f6faab16c">

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/abf50c5c-5ea2-4739-8e05-b798d41d05c6">

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/5ecc20bb-a5e3-464e-a676-578c37762deb">

The method completes:

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/799879be-724c-4365-b775-b967b1015f42">

This method also completes:

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/691d6e9b-8a80-42b8-9235-b92dffeba3a3">

Now we're back in `RemoteAuthenticationHandler.cs`'s `HandleRequestAsync()` method and about to get redirected to our original ReturnUri:

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/ce59159e-c32d-405f-990b-03612c9c5044">

Now we're redirected with the "External" cookie

<img width="900" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/3db98c10-d5d5-4837-b810-a9a389ae8c3a">

#### Run app-level post processing
Now we're in `/Account/ExternalLogin` (In ExternalLogin.razor)

<img width="900" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/b19591c8-47c0-4bae-aa83-31dad84c7e47">

We try to authenticate using "External" cookie to get external user info:

<img width="650" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/9ff77a0c-e3e5-4ec8-8c4a-0f5e6e92295a">

The method looks like this:
<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/6cefbb96-80cf-440e-b0d0-ccb497a72a76">

Now we have userinfo:

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/e535c97e-68c2-40e7-b87c-c1cee4169963">

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/44fce317-969a-4447-ad47-8efd62f17e99">

`ProviderKey` is the Id in Github.

Now we go into `OnLoginCallbackAsync()`:

<img width="650" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/3d043121-946d-44f6-aaa5-f70a9316faa6">

Here:

<img width="750" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/f403c022-4976-46ec-a90b-0187b309a244">

Now we get into `SignInManager.cs`:

Looks like there's this neat method to get user by `loginProvider` (for eg: _github_) and `providerKey` (for eg: _30603497_).

<img width="900" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/9503fe80-ab4a-4efa-843b-d0fc0cc0120f">

Note 1:
---

Whenever `AuthenticateAsync()` method in `AuthenticationService.cs` runs AND `AuthenticateResult.Succeeded` is true, ClaimsTransformation is run.

<img width="900" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/4fe09ea7-7cfd-4a7a-ac8b-f4401b0ff828">

Here:

<img width="900" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/fcd8d092-a4a9-4721-8cbd-47e2fbe10dc0">

Note 2:
---
The action in the query is this:

<img width="1400" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/008bb985-d0a1-409a-b738-3fe277cc2fde">

Also notice the user is not authenticated at this point because we haven't successfully authenticated with the default authentication scheme (Identity.Application).

If for some reason, we're not able to get `externalLoginInfo`, an error message is passed through the cookie in the redirect to be shown in the UI.

<img width="900" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/45b207d1-ad0d-447c-a5aa-e3dd69495ec8">

Like here:

<img width="900" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/bec7b97d-53de-4a31-b4d8-bbfc44e812e4">

The message comes from this component in the `Login.razor` page:

<img width="900" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/ac86c4ae-2f00-4ede-8c8c-3fe59a01c38f">

<img width="600" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/82cda3b4-1fc5-47b2-a308-acce45d1a7d1">

#### Signout "External" cookie and Signin "Application" (primary) cookie
We're still inside `SignInManager.cs`.

Here we Signout external cookie:

<img width="900" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/00031e8f-78b7-42ec-8fdf-4e98d21d4563">

And Signin primary cookie:

<img width="650" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/4365d496-9cdd-44ea-a2a4-f32d41524a13">

By calling this method:

<img width="900" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/21eba0ce-135c-4d40-963b-9606f31b81b5">

Looks like it adds provider name (github) as an additional claim.

<img width="550" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/dbcc552c-2957-4f5f-a6e5-6873c6b6e412">

Then call this method to sign in on primary cookie:

<img width="900" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/8dcee766-d097-49f0-aac3-e95039dc14ff">

Now we're back in `ExternalLogin.razor` by logging in the user successfully.

#### Redirect to final url
<img width="750" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/b73ea334-14d8-4b31-8558-41a7408216e9">

We get to homepage now.

<img width="900" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/9bdb9062-226e-4bc0-9d19-a92595277f75">

Like this:

<img width="900" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/cc1483b7-ac6f-4bf0-bcab-8443825008fa">

Now we hit `AuthenticationMiddleware` and try to authenticate the user. The `Identity.External` cookie is removed and `Identity.Application` cookie is present at this point.

<img width="650" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/01dd551f-657a-4b98-b62c-47e9c4d4dd91">

<img width="850" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/8c12158c-99c0-4f0d-a4ae-871f17846713">

Success!

<img width="900" alt="image" src="https://github.com/akhanalcs/blazor-identity-api/assets/30603497/661fd65a-2a37-4e3c-9947-9d5b2b07aea7">

