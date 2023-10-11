namespace BlazorServerClient.Services;

public class IdentityEndpointService(HttpClient httpClient, IConfiguration configuration)
{
    public async Task<bool> LoginAsync(string? username, string? password)
    {
        try
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }
            
            var response = await httpClient.PostAsJsonAsync("identity/login",
                new { Username = username, Password = password });
            
            // if (!response.IsSuccessStatusCode)
            // {
            //     throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
            // }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        // If you end up here, you failed.
        return false;
    }
    
    public async Task<bool> CreateNewUserAsync(string? username, string? password)
    {
        try
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }
            
            var response = await httpClient.PostAsJsonAsync("identity/register",
                new { Username = username, Password = password });
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        // If you end up here, you failed.
        return false;
    }
}