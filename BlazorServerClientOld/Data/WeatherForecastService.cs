namespace BlazorServerClient.Data;

public class WeatherForecastService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    
    private const string ServiceName = "MyProtectedWebAPI"; // Name coming from: .AddDownstreamApi("MyProtectedWebAPI", builder.Configuration.GetSection("ProtectedWebAPI"))

    public WeatherForecastService(HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }
    
    public async Task<IEnumerable<WeatherForecast>?> GetForecastAnotherWayAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/weather");
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Invalid status code in the HttpResponseMessage: {response.StatusCode}.");
            }
            
            var forecasts = await response.Content.ReadFromJsonAsync<IEnumerable<WeatherForecast>>();
            return forecasts;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        // If you end up here, you get nothing.
        return Enumerable.Empty<WeatherForecast>();
    }
}