public class AfricasTalkingService
{
    private readonly HttpClient httpClient;
    private readonly string? username;

    public AfricasTalkingService(IConfiguration config)
    {
        var baseUrl = config["AfricasTalking:BaseUrl"];
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentNullException("AfricasTalking:BaseUrl", "Base URL configuration is missing or empty.");

        httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };
        httpClient.DefaultRequestHeaders.Add("apiKey", config["AfricasTalking:ApiKey"]);
        username = config["AfricasTalking:Username"];
    }

    public async Task<string> SendSmsAsync(string to, string message)
    {
        var payload = new Dictionary<string, string>
        {
            { "username", username ?? string.Empty },
            { "to", to },
            { "message", message }
        };

        var response = await httpClient.PostAsync("/version1/messaging", new FormUrlEncodedContent(payload));
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
