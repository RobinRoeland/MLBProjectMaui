using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Maui.Storage;

public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Method to authenticate the user and obtain a JWT token
    public async Task<string> LoginAsync(string email, string password)
    {
        var loginModel = new
        {
            Email = email,
            Password = password
        };

        var jsonContent = JsonConvert.SerializeObject(loginModel);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var url = "https://localhost:5204/api/Account/Login"; // Ensure this is HTTPS

        try
        {
            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                // Parse the response to get the token
                var responseString = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonDocument.Parse(responseString).RootElement;

                if (jsonResponse.TryGetProperty("token", out var tokenElement))
                {
                    // Get the token
                    return tokenElement.GetString();
                }
            }
            else
            {
                // Handle failed login (e.g., invalid credentials)
                var errorResponse = await response.Content.ReadAsStringAsync();
                // You could parse and show a detailed error message here
                return null;
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions (e.g., network issues)
            return null;
        }

        return null;
    }
}