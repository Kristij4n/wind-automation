using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications; // Install via NuGet

const string apiKey = "8ae53109d163e2407bd4125a18ef869e";
const string city = "Čačinci";
const double strongWindThreshold = 10.0; // m/s, adjust as needed
const int checkIntervalMinutes = 10; // Check every 10 minutes

// Entry point for async code in .NET 8
await MonitorWindAsync();

async Task MonitorWindAsync()
{
    while (true)
    {
        await MainAsync();
        await Task.Delay(TimeSpan.FromMinutes(checkIntervalMinutes));
    }
}

async Task MainAsync()
{
    using var http = new HttpClient();
    var url = $"https://api.openweathermap.org/data/2.5/weather?q={Uri.EscapeDataString(city)}&appid={apiKey}&units=metric";
    try
    {
        var response = await http.GetFromJsonAsync<WeatherResponse>(url);

        if (response?.Wind?.Speed >= strongWindThreshold)
        {
            var toastContent = new ToastContentBuilder()
                .AddText($"Strong wind alert for {city}!")
                .AddText($"Current wind speed: {response.Wind.Speed} m/s")
                .GetToastContent();

            var toastNotification = new ToastNotification(new Windows.Data.Xml.Dom.XmlDocument());
            toastNotification.Content.LoadXml(toastContent.GetContent());
            ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
        }
        else
        {
            Console.WriteLine($"Wind is normal at {city}. Current speed: {response?.Wind?.Speed ?? 0} m/s");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error checking wind: {ex.Message}");
    }
}

// Weather API response classes
public class WeatherResponse
{
    [JsonPropertyName("wind")]
    public WindInfo Wind { get; set; }
}

public class WindInfo
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }
}