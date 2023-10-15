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

## Add Microsoft Authentication to your app. [Reference[(https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/microsoft-logins?view=aspnetcore-8.0).

### Create an app in Microsoft Developer Portal (Azure)
<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/ecb5b07c-63d1-4dd0-9eff-2bf0faa4eb42">

### Store your config
Store the secret in `user-secrets`. Store ClientId in appsettings.json.

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/7fe8b6ed-1e3e-4ee6-96a1-b3a1a343b1b2">

### Add Nuget package
<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/076100a7-876b-43aa-9757-09deac4c11ed">

### Setup `Program.cs`
<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/930f4184-26f4-4fdb-a6e1-ca7e84790eb8">

## Take OAuth authentication for a test drive

1. Click "Microsoft"
   
   <img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/4e9d7a36-78c9-4259-a708-0048e16207c3">

2. Give consent
   
   <img width="450" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/d68ae718-d4b9-4925-bf6a-51c3ac12df26">

3. These are the claims, MSFT sends to the app

   <img width="550" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/77512a98-c156-47de-a7c1-5fe27adb2e03">
   
## How Microsoft Authentication is setup under the covers
Command + Click on `.AddMicrosoftAccount` method:

<img width="350" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/ef6ff0ff-9b6b-43e0-a39d-28ea90500e8f">

Check what `AuthenticationScheme` was used:

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/674d6247-cb1c-4e43-9816-e4d125b7a792">

By going to `MicrosoftAccountDefaults`:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/fdbe90fb-286c-46c1-8d2b-0e07f4015a49">

You can see `AuthenticationScheme` used was `"Microsoft"` and also see the 3 most important endpoints in OAuth: `AuthorizationEndpoint`, `TokenEndpoint` and `UserInformationEndpoint`.

## Add GitHub authentication to your app. [Reference](https://youtu.be/PUXpfr1LzPE?si=LTlb0vyOqLXPiQ4t).

### Register an app in Github
<img width="550" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/b0a2fc52-b8a5-4e63-8b20-a18944848b64">

Grab clientid and clientsecret.

### Go to GitHub's OAuth docs to find 3 important endpoints as part of auth: "authorize endpoint" where we obtain the one time code, "token endpoint" to get access token and "user info endpoint" to get user info. [Reference](https://docs.github.com/en/apps/oauth-apps/building-oauth-apps/authorizing-oauth-apps).

1. Authorize
   
   <img width="350" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/77d4ecd3-d5aa-4a42-829f-46f6498fbeec">
   
   After the user is logged in, GitHub sends us the one time code (that can be used to exchange for a token) to the redirect url that we set during registration.
   
   `https://localhost:7074/signin-github`
   
3. Token
   
   <img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/4f740b74-57b0-47bc-89a0-e71e34de849b">

4. User Information endpoint to get user info
   
   <img width="450" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/ddc26219-7642-4501-878b-625d3620af5c">

### Store your config
<img width="550" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/4dbaf4b6-b43b-4412-9be3-bd09e6dd3089">

### Setup `Program.cs`
<img width="950" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/8029a583-add8-4842-8e18-9f588ad5c641">

## Taking a look at Authentication middleware
Every time you navigate to ANY page in the app, the `Authentication` middleware runs (**It's middleware duh!**).
It's the bit that's inside `app.UseAuthentication`:

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/8c89eac8-2b5f-4ae0-8840-94da5bd0e3bd">

This runs:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/061212b0-6a3b-4680-830e-cd4f246a2426">

### Handlers:
See how `IAuthenticationHandler` looks like:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/01b3b931-2628-4f28-a25a-3143f2f34503">

See how `IAuthenticationRequestHandler` looks like (this is relevant in `var handler = await handlers.GetHandlerAsync(context, scheme.Name) as IAuthenticationRequestHandler;` line shown below):

<img width="550" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/958e94de-0abc-4d23-90e8-4272edde5a6a">

---

I have setup Microsoft and Github (OAuth) authentication, so I've got 2 handlers now:

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/99bbc7d8-27c2-4c5f-87cd-72c7fe71ef84">

Those 2 handlers come from the service registration section:

<img width="250" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/dac16155-7c29-413c-8f5b-9dac327dc2de">

The middleware determines if it should handle auth request on those handlers (using `IAuthenticationRequestHandler.HandleRequestAsync`). 

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/6daaf501-1804-4f26-807c-5eee61705fe1">

For eg: For my case (Microsoft, GitHub handlers), it's determined by `HandleRequestAsync()` in `RemoteAuthenticationHandler.cs`:

<img width="450" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/c2844e22-93ec-474c-9b5e-fed5a897d81b">

Since, I'm going to "/counter" page now, the `HandleRequestAsync` method short circuits.

### DefaultAuthentication:
Default AuthenticationScheme is whatever I setup in `.AddAuthentication`:

<img width="450" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/6019d4bb-c0a0-430b-8bcf-bc7bee007bbd">

Using the default authentication scheme, it tries to authenticate the current request. If it succeeds, you get `.User` in the HttpContext.

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/e018d9a7-88cd-4443-bd18-b77d69da11bd">

To view the handler for your default auth scheme, navigate from here:

<img width="550" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/c4d0281d-ee29-4886-90ef-f923537385f6">

To here:

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/a358222d-cc17-4d80-9a1f-016f35cc496e">

To here:

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/d94e3e07-8435-4c39-83e9-70ef66b7756b">

To here:

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/3e122c7a-0322-4b8b-8bd0-042a56f09b2b">

To finally here:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/ec95dcf2-9209-4801-a89d-2aab2f50428c">

Here you can see that this service has Schemes, Handlers etc. to authenticate a request.

Now let's get back to how this line executes:

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/230d9e13-1e78-4892-9ff3-8877a10b574b">

It just calls `.AuthenticateAsync` on the `AuthenticationService`:

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/c11a1bbe-131d-4777-98b3-9910f34cdfba">

#### Let's see this in action:
See the SchemeName and Handler:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/d7220f34-d95e-437b-8fe6-ed7c2be51555">

Now we get into `AuthenticateService`'s `AuthenticateAsync` method:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/5b4e3109-3aac-4030-b583-129f18ab6295">

Then into `AuthenticationHandler`'s `AuthenticateAsync` method:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/360f549f-e8a6-400e-aebc-346496e29b9f">

Then into `AuthenticationHandler`'s `HandleAuthenticateOnceAsync` method:

<img width="450" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/2d5fb0e8-a0ff-4826-b38a-0d44b87f0f67">

Then into `AuthenticationHandler`'s `HandleAuthenticateAsync` method:

<img width="450" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/00c8d5d7-062c-4266-8fb9-4d66c5c5e52d">

It's an abstract method that is implemented in a class that derives it, `CookieAuthenticationHandler` in this case, so we end up here:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/b91dede3-889a-431c-b586-1de877bb1ad8">

Then we finally get this result:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/cf863f30-b60b-4605-809f-c653cab8b4da">

## Taking a look at Microsoft Authentication in detail
<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/917e57cb-8608-433d-b6d0-3de029224c29">

After we click on "Microsoft" to login, the following steps happen:

### Calling `AuthorizationEndpoint` endpoint

We end up with this POST in `ExternalLoginPicker.razor`:

<img width="950" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/b44a5ae3-7466-4f47-b7b0-30b39d59abe1">


**Request in Network tab shows POST request with Antiforgery cookie:**

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/b4ec46b4-d31f-483c-97b1-779b0ac64e70">

We land in `AuthenticationMiddleware`:

`CookieAuthenticationHandler` tries to authenticate, but it doesn't succeeed:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/1011541b-aa0f-4d8f-835b-16a0c83201e8">

<img width="500" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/d915b8ec-2a33-4d5a-a878-7ef19bbfcd9c">

This pass doesn't yield a user and we go to next middleware:

<img width="280" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/02e60523-b511-4d92-905a-bb904c2aa231">

---

Now we land in `AuthenticationService.cs`'s `ChallengeAsync` method:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/bec3200a-6962-4b5c-bacb-36c5caf2d5f5">

The `scheme` is "Microsoft".

The `HttpContext` looks like this:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/de1e973a-f711-494b-8d47-b169ba68ffec">

Now we land in `AuthenticationHandler.cs`'s `ChallengeAsync` method:

<img width="450" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/b5730980-eeb6-4c1c-8612-c916e53f5a5d">

Now we land in `OAuthHandler.cs`'s `HandleChallengeAsync` method:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/794934ab-1c21-4edb-abcf-edca542aff6a">

Since `BuildChallengeUrl` method is overridden in `MicrosoftAccountHandler`, we end up here for that call:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/260f4330-62a3-4c27-898c-a5afd4d3e98e">

The state is created from `AuthenticationProperties`:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/98c6bffa-9d49-4918-9d2f-85fdb8535eaa">

`state`:

<img width="950" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/19fd16d9-aa5c-4750-9930-118443d54715">

`queryStrings`

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/9269e913-518c-491a-83e2-48cc9389c148">

The `authorizationEndpoint` in `OAuthHandler.cs`'s `HandleChallengeAsync` method:

`https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id=0c0e0f34-2e24-4d19-916e-e5f058a01b20&response_type=code&redirect_uri=https://localhost:7074/signin-microsoft&scope=https://graph.microsoft.com/user.read&code_challenge=gqdo4sZaQzm6pPjclYtNIqX-7WdMGvUOCvpYqgMrwUo&code_challenge_method=S256&state=CfDJ8L8UGyfUjDBErg6qYS34nXCdYW31rFDIH6SbFMsWc5UWTefX9kxzVIvSnVVwi0WKZWaXVMntAWvK6OGMk6q4HkaTtPYlcGQtFbGD5NuIKYPiCiVUX9aYHp8UGXqzws62fUDvBuStWWA9WclWpaQa44T0DPgEecY46IQHJ3NuPJsx2ifWSWGTf2zMSQEYgm8taSEuyQoKeSyo1cSNbEymoCmv2kVXBQrFQajnCz0bXCXbbi7l1lH4y88oclvXwPZnaQ7ra1eMkGma0lYs4IYWodNyPkCw48_Fz5dcQgxLeHZBTkAKRMRJJvBEvTyZzUWBdtcsqzrEQrY-pe1YZZ5b8h-s3cc2a1Bihas9yItJ1umk8Psb8AKJ4m7UjV76gS0jTazpX3njQcVi4mKE31THn5A`

At this point we raise event to redirect to authorization endpoint:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/2dab6bfb-008f-495a-ae13-5626beccc876">

The location is:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/ef4bff87-4f5e-4e31-b8de-aaf0ba1efdbb">

`https://login.microsoftonline.com/common/oauth2/v2.0/authorize?client_id=0c0e0f34-2e24-4d19-916e-e5f058a01b20&response_type=code&redirect_uri=https://localhost:7074/signin-microsoft&scope=https://graph.microsoft.com/user.read&code_challenge=gqdo4sZaQzm6pPjclYtNIqX-7WdMGvUOCvpYqgMrwUo&code_challenge_method=S256&state=CfDJ8L8UGyfUjDBErg6qYS34nXCdYW31rFDIH6SbFMsWc5UWTefX9kxzVIvSnVVwi0WKZWaXVMntAWvK6OGMk6q4HkaTtPYlcGQtFbGD5NuIKYPiCiVUX9aYHp8UGXqzws62fUDvBuStWWA9WclWpaQa44T0DPgEecY46IQHJ3NuPJsx2ifWSWGTf2zMSQEYgm8taSEuyQoKeSyo1cSNbEymoCmv2kVXBQrFQajnCz0bXCXbbi7l1lH4y88oclvXwPZnaQ7ra1eMkGma0lYs4IYWodNyPkCw48_Fz5dcQgxLeHZBTkAKRMRJJvBEvTyZzUWBdtcsqzrEQrY-pe1YZZ5b8h-s3cc2a1Bihas9yItJ1umk8Psb8AKJ4m7UjV76gS0jTazpX3njQcVi4mKE31THn5A`

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/2548df88-a5ea-4b44-8dc5-4148e487ff41">

Look at this:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/791b8e46-2bfd-41ab-b420-2ec2b6e73ae0">

We're redirected to "authorize endpoint".

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/c1afe7c5-07ab-4785-bb6c-a77d3d290011">

### Calling `TokenEndpoint` endpoint
We land on this page:

<img width="400" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/da6edbd3-9a02-418c-9753-5597b789e7f3">

I hit continue. I land in `AuthenticationMiddleware`:

Notice, there's code:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/2457403f-8243-4378-971d-50b7d22df69e">

Do I need this? If not remove.

<img width="950" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/3615456b-e4b9-4004-8f0b-a8cfae0808b3">

Now we're in `RemoteAuthenticationHandler.cs`'s `HandleRequestAsync` method:

<img width="500" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/c735eef0-9497-4354-88f1-56a0e1ff4865">

Now we're in `OAuthHandler.cs`'s `HandleRemoteAuthenticateAsync` method:

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/9d7c30dc-25b2-4bbf-a278-f7403a6239bf">

State can be up-protected to view `AuthenticationProperties`:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/45ec739c-ed79-4501-bcfa-f75a4344b2dc">

### Calling `TokenEndpoint` endpoint
Exchanging code for a token happens here in `OAuthHandler.cs`:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/63d5aadb-0df4-48d8-81af-1d8ee66b110b">

The identity:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/74bbd0a5-7c12-4b01-b75d-d4f55cc4d751">

Create Ticket:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/d90e4e92-7111-4cb0-80f1-f69e2891ec1d">

### Calling `UserInformation` endpoint
Now we're in `MicrosoftAccountHandler.cs`'s `CreateTicketAsync` method:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/20edddb9-4ef3-4e4f-ae51-9cfc7eb3bbe0">

After the call I get these Claims:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/0b41bf8f-4b0f-437c-992f-b189bb11d74f">

### Signing In the user with `Identity.External` scheme
Now we're back in `RemoteAuthenticationHandler.cs`'s `HandleRequestAsync` method.

SignIn the user under External scheme now (The debugger just skipped past it, I don't know if it actually worked):

<img width="500" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/3756116c-f709-463e-b40b-62b6c6ed09fc">

I don't see the cookie, so I don't think this worked.

Redirect:

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/cf0cd2fd-d988-432f-92d7-2697fb50281a">

---

Back to default authentication scheme -> Authentication middleware runs to completion without any cookie set this time.

---
---

Back to `CookieAuthenticationHandler.cs`'s `HandleAuthenticateAsync` method and `result.Succeeded` is true this time.

Scheme is External

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/5fab4bdf-2650-4e37-afcc-570fa5981636">

Now we're returning from `AuthenticationService.cs`'s `AuthenticateAsync` method:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/d0ecd693-a166-4a6f-a0b0-8af5d2757ba6">

Now we're in `SignInManager.cs`'s `GetExternalLoginInfoAsync` method. Provider is "Microsoft".

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/79959f2d-a5fa-4c7c-9d89-c5b52c01ebdf">

Return `ExternalLoginInfo`:

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/6750b0b1-b8cd-4b4e-ba12-b93b0acdecc1">

Now we're in `ExternalLogin.razor`:

<img width="550" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/0b3d0e2c-2c3b-402d-9f7e-ae3c2de1aa7c">

<img width="550" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/dec7eaf9-699b-4a68-b526-805a1cc20f31">

Sign in the user with this external login provider:

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/a74a3718-fcff-41d4-be7d-32157c40ec5f">

It fails.

This is where users see prompt to enter email:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/4b434c86-252f-460b-ba6e-6153206ca617">

---

Final scheme:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/1ee135ee-02b3-4f69-93f6-c4ca92c0a6ed">

Doesn't succeed.

Now fill up the form and register:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/c5760ed0-bc6d-4b6a-a0d9-4619c9e727b9">






Middleware -> 

### The `await handler.HandleRequestAsync()` will return true
In this case, any subsequent middleware won't execute.

### Call `AuthorizationEndpoint` endpoint
#### Now we get into `RemoteAuthenticationHandler.cs`'s `HandleRequestAsync` method

#### Now we get into `OAuthHandler.cs`'s `HandleRemoteAuthenticateAsync` method

Check out the state from the query string.

### Call `TokenEndpoint` endpoint
#### This happens in `OAuthHandler.cs`'s `HandleRemoteAuthenticateAsync` method

### Create Identity

### Call `UserInformation` endpoint
#### Create AuthenticationTicket. Note the scheme here.

### Get back to `RemoteAuthenticationHandler.cs`'s `HandleRequestAsync` method
1. Sign In the user using external cookie
   
   <img width="1665" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/6c4eefef-a424-4439-9943-ef28240c0464">

2. Redirect

### Form to get registered into the app

### Sign in again
Finally Sign In the user into the app using default authentication scheme

---

To see how the above claims were fetched, you can see it in the `MicrosoftAccountOptions` class added from the package. Here you can see that it had asked for the scope of `user.read` and Claims were mapped this way:
      
<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/82aa93ac-f81f-4c36-988e-9c9228c2512c">


1. Step 1: Call `AuthorizationEndpoint` endpoint
2. Step 2: Call `TokenEndpoint` endpoint
3. Step 3: Call `UserInformationEndpoint`
   2. The user information is fetched inside `CreateTicketAsync` method in `MicrosoftAccountHandler`:
      
      <img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/0009ca7e-dac3-474e-964b-bde58dfefdf4">
      
   3. You can view `MicrosoftAccountOptions` and `MicrosoftAccountHandler` in `MicrosoftAccountExtensions`. Great information here. Check it out!
      
      <img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/840aaa47-45a2-41ec-89aa-1a9078c3d411">
  
   4. 
   
### How the claims showed up in the UI
The services are finally setup at the last line of `MicrosoftAccountExtensions` where there's a call to `.AddOAuth`.
Here you can see the `MicrosoftAccountHandler`.

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/f93e19a0-e5b7-4380-8497-9a75b7e7c088">

See what an `AuthenticationTicket` is made of:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/dba23d54-4fca-4f7c-b725-2ec77ef2548b">

See what properties are in `AuthenticationProperties`. This stores state values of `AuthenticationTicket`. It gets passed around in the query string.

<img width="500" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/dda511fe-80f2-4144-81a9-4673c4ccfdf8">

For eg: It's read in `HandleRemoteAuthenticateAsync` method in `OAuthHandler.cs`:

<img width="450" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/1f6f73c2-7695-46b6-a631-e7784bc997c0">

See how `OAuthTokenResponse` looks like:

<img width="500" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/5f83c9de-b287-4c13-a687-f42d88a3a0d5">

Now check out how the handler looks like in `MicrosoftAccountHandler.cs`

<img width="900" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/ce52b7e2-5673-48c9-aff3-0a754f2bf4c0">

---



See how `RemoteAuthenticationHandler` looks like. This is what calls to Microsoft.

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/1f1a624c-0ad2-450a-a315-7ab3cc5c3742">

Here, pay attention to this method `ShouldHandleRequestAsync` which returns true only when the `Options.CallbackPath` that you provide matches with the `Request.Path`.

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/41a53b93-2a7d-44fa-80c1-3a4c4d3c4509">

Check out this important method in `RemoteAuthenticationHandler.cs`:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/4e355d64-04d7-423b-b8ee-7381cd54b94a">

`HandleRemoteAuthenticateAsync` is abstract method in this class so whichever class inherits from it should implement it.
In this case `OAuthHandler` class inherits it and implements it.
