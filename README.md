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

## Add Microsoft Authentication to your app. [Reference](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/microsoft-logins?view=aspnetcore-8.0).

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

## How the claims showed up in the UI
The services are finally setup at the last line of `MicrosoftAccountExtensions` where there's a call to `.AddOAuth`.
Here you can see the `MicrosoftAccountHandler`.

<img width="950" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/f93e19a0-e5b7-4380-8497-9a75b7e7c088">

To see how the above claims were fetched, you can see it in the `MicrosoftAccountOptions` class added from the package. Here you can see that it had asked for the scope of `user.read` and Claims were mapped this way:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/82aa93ac-f81f-4c36-988e-9c9228c2512c">

## How Microsoft Authentication is setup under the covers
Command + Click on `.AddMicrosoftAccount` method:

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/ef6ff0ff-9b6b-43e0-a39d-28ea90500e8f">

Check what `AuthenticationScheme` was used:

<img width="950" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/674d6247-cb1c-4e43-9816-e4d125b7a792">

By going to `MicrosoftAccountDefaults`:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/fdbe90fb-286c-46c1-8d2b-0e07f4015a49">

You can see `AuthenticationScheme` used was `"Microsoft"` and also see the 3 most important endpoints in OAuth: `AuthorizationEndpoint`, `TokenEndpoint` and `UserInformationEndpoint`.

## Add GitHub authentication to your app. [Reference](https://youtu.be/PUXpfr1LzPE?si=LTlb0vyOqLXPiQ4t).

### Register an app in Github
<img width="550" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/b0a2fc52-b8a5-4e63-8b20-a18944848b64">

Grab `clientid` and `clientsecret`.

### Go to GitHub's OAuth docs to find 3 important endpoints as part of OAuth: Authorize, Token and User endpoints.[Reference](https://docs.github.com/en/apps/oauth-apps/building-oauth-apps/authorizing-oauth-apps).

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
Just look at the code.

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

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/958e94de-0abc-4d23-90e8-4272edde5a6a">

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

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/6019d4bb-c0a0-430b-8bcf-bc7bee007bbd">

Using the default authentication scheme, it tries to authenticate the current request. If it succeeds, you get `.User` in the HttpContext.

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/e018d9a7-88cd-4443-bd18-b77d69da11bd">

To view the handler for your default auth scheme, navigate from here:

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/c4d0281d-ee29-4886-90ef-f923537385f6">

To here:

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/a358222d-cc17-4d80-9a1f-016f35cc496e">

To here:

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/d94e3e07-8435-4c39-83e9-70ef66b7756b">

To here:

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/3e122c7a-0322-4b8b-8bd0-042a56f09b2b">

To finally here:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/ec95dcf2-9209-4801-a89d-2aab2f50428c">

Here you can see that this service has Schemes, Handlers etc. to authenticate a request.

Now let's get back to see how this line executes:

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

## Taking a look at GitHub Authentication in detail
<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/917e57cb-8608-433d-b6d0-3de029224c29">

Go ahead and click "OAuth" button.

### Setup for the `AuthorizationEndpoint` call

We end up with this POST in `ExternalLoginPicker.razor`:

<img width="950" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/b44a5ae3-7466-4f47-b7b0-30b39d59abe1">

**Request in Network tab shows POST request with Antiforgery cookie:**

<img width="950" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/8b87c997-ba8b-410c-930a-f5f95c52d135">

We land in `AuthenticationMiddleware`.

Now we land in `AuthenticationService.cs`'s `ChallengeAsync` method:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/bec3200a-6962-4b5c-bacb-36c5caf2d5f5">

The `scheme` is "github".

The `HttpContext` looks like this:

<img width="800" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/70573903-ff18-403e-8788-feb62ea50dbe">

Now we land in `AuthenticationHandler.cs`'s `ChallengeAsync` method:

<img width="450" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/b5730980-eeb6-4c1c-8612-c916e53f5a5d">

Now we land in `OAuthHandler.cs`'s `HandleChallengeAsync` method:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/794934ab-1c21-4edb-abcf-edca542aff6a">

We end up in `BuildChallengeUrl`:

<img width="700" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/1a2279d8-981a-4fa6-acbc-69ae8e727696">

[This is example during call to Microsoft and not GitHub that this example is exploring]
The state is created from `AuthenticationProperties`:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/98c6bffa-9d49-4918-9d2f-85fdb8535eaa">

`state`:

<img width="950" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/19fd16d9-aa5c-4750-9930-118443d54715">

`queryStrings`

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/9269e913-518c-491a-83e2-48cc9389c148">

The `authorizationEndpoint` in `OAuthHandler.cs`'s `HandleChallengeAsync` method:

`https://github.com/login/oauth/authorize?client_id=528e421d4ac95a87b883&scope=&response_type=code&redirect_uri=https://localhost:7074/signin-github&state=CfDJ8L8UGyfUjDBErg6qYS34nXCpL9Pc9wE9AAGGU_sKu7MOrh-IGtQMsH6nw9stVoxz5QvwgDE75XeHR3OKTQbV1PgbAJSPhsFk5docLrGICnvu4WuEe3GOoaRGFmveBBJTp7mty5-IvG58cVS_l5fg1RtfIJMc74jTL0NdMZWb67mUR6D_72iae_hGajnsA5-yAmfmeuVopdKgoKgMSLRWqcBCcbiJndJELPfn21MInqhVRf6ptsDxnlVj-ACapPwkQjSDvNelAkDxe-er5kbgEVFVvG-3m2CshC_FbJjVmFVR8XER-1jNsUD2buS6w28qFg`

At this point we raise event to redirect to authorization endpoint:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/2dab6bfb-008f-495a-ae13-5626beccc876">

The location is:

`https://github.com/login/oauth/authorize?client_id=528e421d4ac95a87b883&scope=&response_type=code&redirect_uri=https://localhost:7074/signin-github&state=CfDJ8L8UGyfUjDBErg6qYS34nXCpL9Pc9wE9AAGGU_sKu7MOrh-IGtQMsH6nw9stVoxz5QvwgDE75XeHR3OKTQbV1PgbAJSPhsFk5docLrGICnvu4WuEe3GOoaRGFmveBBJTp7mty5-IvG58cVS_l5fg1RtfIJMc74jTL0NdMZWb67mUR6D_72iae_hGajnsA5-yAmfmeuVopdKgoKgMSLRWqcBCcbiJndJELPfn21MInqhVRf6ptsDxnlVj-ACapPwkQjSDvNelAkDxe-er5kbgEVFVvG-3m2CshC_FbJjVmFVR8XER-1jNsUD2buS6w28qFg`

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/2548df88-a5ea-4b44-8dc5-4148e487ff41">

### Calling `AuthorizationEndpoint` endpoint
We're redirected to "authorize endpoint" from the first call.

<img width="950" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/ac3af5f0-bc25-4119-8d40-81137e93a278">

Auth server puts the code in the query string and sends a redirect:

<img width="950" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/3e4d89db-7e01-40f4-93a1-a1e2a251a588">

### Calling `TokenEndpoint`
And now we have a code:

<img width="950" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/bd646f9e-8b7a-46dc-9f9e-88b0ccf1703f">

We grab this in `OAuthHandler.cs`'s `HandleRemoteAuthenticateAsync` method:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/2ee39b98-29a2-4021-b0d4-961d3ad85c2f">

State can be un-protected to view `AuthenticationProperties`:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/48fe861e-24e9-4b5c-a416-64e2f056ce35">

#### Exchanging code for a token happens here in `OAuthHandler.cs`:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/bb770630-0068-40af-b4db-012f1cb73cd8">

The identity:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/aecf1c47-3330-4eb4-89f9-e9a59eb4870d">

### Calling `UserInformation` endpoint
Create Ticket:

<img width="450" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/90801618-cfcf-4466-83ac-a4509b54efef">

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/1b72f629-f693-4416-8746-5243d05d0821">

We have overridden this, so we end up here:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/05377106-ade5-43e8-ad1f-e0ad7256de3a">

After the call, I get these 2 Claims:

<img width="900" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/2a0e1618-fb33-4261-8b82-14afe1114efa">

### Signing In the user with `Identity.External` scheme
After the code exchange, we're directed:

<img width="950" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/0d0139bd-723b-4b21-a969-8c840a76c940">

At this point, we already have `External` cookie set.

### Signing In the user with `Identity.Application` scheme
Now I'm in `ExternalLogin.razor` to get Signed in:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/6c589021-f374-42fc-8eeb-c45ed7cded16">

Now I've got Application cookie as well:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/ca5d7492-c3a4-4c1f-8e03-571f874cd6c0">

Now I'm redirected to home page:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/a7988cb8-0790-4bca-b54d-61517a8cb48a">

Like so:

<img width="750" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/16aebef7-2161-40a1-944b-d46d15898902">

Cookie is set:

<img width="650" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/410820d9-2fea-4ded-9e28-1c10ed0e4c3a">

Now the homepage looks pretty neat:

<img width="850" alt="image" src="https://github.com/affableashish/blazor-identity-api/assets/30603497/9fc58ba7-9fad-4ca2-9c38-89da1fb598e1">
