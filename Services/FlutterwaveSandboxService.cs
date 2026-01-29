public class FlutterwaveSandboxService
{
    private readonly HttpClient httpClient;

    public FlutterwaveSandboxService(IConfiguration config)
    {
        httpClient = new HttpClient
        {
            BaseAddress = new Uri(config["Flutterwave:BaseUrl"] ?? throw new InvalidOperationException("Flutterwave:BaseUrl configuration is missing"))
        };
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", config["Flutterwave:SecretKey"]);
    }

    public async Task<string> CreateTestPaymentAsync(string email, decimal amount)
    {
        var payload = new
        {
            tx_ref = Guid.NewGuid().ToString(),
            amount,
            currency = "KES",
            redirect_url = "https://yourapp.com/payment/callback",
            customer = new { email }
        };

        var response = await httpClient.PostAsJsonAsync("/payments", payload);
        return await response.Content.ReadAsStringAsync();
    }
}
