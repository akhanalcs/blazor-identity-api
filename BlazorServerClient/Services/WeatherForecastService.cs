using BlazorServerClient.Data;

namespace BlazorServerClient.Services;

public class WeatherForecastService(HttpClient httpClient)
{
    public async Task<IEnumerable<WeatherForecast>?> GetForecastAnotherWayAsync()
    {
        try
        {
            var response = await httpClient.GetAsync("/weather");
            
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