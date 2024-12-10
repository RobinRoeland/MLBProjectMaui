using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BaseballScoringApp.Models;
using BaseballScoringApp.Services;
using Newtonsoft.Json;

namespace BaseballScoringApp
{
    public partial class MainPageLogin : ContentPage
    {
        BBDataRepository repo;
        HttpClient _httpClient;
        public MainPageLogin()
        {
            InitializeComponent();
            repo = BBDataRepository.getInstance();
            _httpClient = new HttpClient();
            JwtService.DeleteTokenAsync();
            EmailEntry.Text = "admin@testemail.com";
            PasswordEntry.Text = "Start123#";
        }
        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text;
            string password = PasswordEntry.Text;


            /*repo = BBDataRepository.getInstance();
            await repo.InitialiseOnLogin();
            return;
            */
            // Basic authentication logic (replace with real authentication)
            if (AuthenticateUser(email, password).Result)
            {
                // Now dismiss the login modal and navigate to the main page
                await DismissLoginModal();

                // Set the MainPage to AppShell after the modal is dismissed
                Application.Current.MainPage = new AppShell(); // This ensures that AppShell becomes the root page
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                // Show an error message to the user if login fails -> handled inside AuthenticateUser
                //await DisplayAlert("Login failed", "Invalid username or password.", "OK");
            }
        }

        private async Task<bool> AuthenticateUser(string email, string password)
        {
            // Check if fields are empty
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                // Show error message if fields are empty
                ErrorLabel.Text = "Both Email and Password are required.";
                ErrorLabel.IsVisible = true;
                return false;
            }

            // Hide the error message once the user enters both fields
            ErrorLabel.IsVisible = false;

            // Check if user exists in the database is done in the AccountController
            // Create the login model
            var loginModel = new LoginModel
            {
                Email = email,
                Password = password
            };

            // Convert the login model to JSON
            var jsonContent = JsonConvert.SerializeObject(loginModel);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var url = Globals.serverURL + "/api/Account/Login";

            //_httpClient.Timeout = TimeSpan.FromSeconds(30); // Set a timeout (30 seconds, adjust as needed)

            try
            {
                // Send the POST request
                var response = await _httpClient.PostAsync(url, content).ConfigureAwait(false); // Adding ConfigureAwait(false) ensures task completion doesn't block UI

                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a stream
                    var result = await response.Content.ReadAsStringAsync();
                    // Parse the response to get the authentication and the JWT token
                    var isAuthenticated = JsonDocument.Parse(result).RootElement
                                           .TryGetProperty("isAuthenticated", out JsonElement isAuthenticatedElement);
                    var token = JsonDocument.Parse(result).RootElement
                                           .TryGetProperty("token", out JsonElement tokenElement);

                    // Check if authentication was successful
                    if (isAuthenticatedElement.GetBoolean())
                    {
                        // Store the JWT securely in SecureStorage
                        JwtService.SaveTokenAsync(tokenElement.GetString());
                        Globals.loggedIn = true;

                        return true;
                    }
                    else
                    {
                        // If unsuccessful, show an error message
                        // Handle specific error messages returned from the server
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        var errorMessage = JsonDocument.Parse(errorResponse).RootElement
                            .TryGetProperty("message", out JsonElement messageElement) ? messageElement.GetString() : "An unknown error occurred";

                        // Use MainThread.BeginInvokeOnMainThread in MAUI to safely update
                        //  the UI from a background non-UI thread to avoid an exception
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            ErrorLabel.Text = string.IsNullOrEmpty(errorMessage) ? "Unknown error in login." : errorMessage;
                            ErrorLabel.IsVisible = true;
                        });
                    }
                }
                else
                {
                    // Handle error (e.g., server not reachable)
                    //await DisplayAlert("Error", "Unable to reach the server. Please try again later.", "OK");

                    //var responseBody = await response.Content.ReadAsStringAsync();
                    // Log or display the body of the failed response
                    //await DisplayAlert("Error", $"Request failed: {responseBody}", "OK");

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert("Error", $"Request failed with status: {response.StatusCode}", "OK");
                    });
                }
            }
            catch (HttpRequestException e)
            {
                // Log the detailed exception for better debugging
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Exception", $"An error occurred: {e.Message}", "OK");
                });
            }
            catch (Exception e)
            {
                // Catch any other exceptions
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("General Error", $"Unexpected error: {e.Message}", "OK");
                });
            }

            return false;
        }

        // Helper method to dismiss the modal login-page
        private async Task DismissLoginModal()
        {
            try
            {
                // Check if current MainPage is a NavigationPage (it should be)
                if (Application.Current.MainPage is NavigationPage navigationPage)
                {
                    var modalStack = navigationPage.Navigation.ModalStack;

                    // Check if there's a modal to pop
                    if (modalStack.Count > 0)
                    {
                        Debug.WriteLine("Popping modal from NavigationPage...");
                        await navigationPage.Navigation.PopModalAsync();
                    }
                    else
                    {
                        Debug.WriteLine("No modal to dismiss from NavigationPage.");
                    }
                }
                else
                {
                    // Fallback: Attempt to pop from Shell if current MainPage isn't a NavigationPage
                    Debug.WriteLine("MainPage is not a NavigationPage. Trying to pop modal from Shell or fallback.");
                    if (Shell.Current != null)
                    {
                        var shellModalStack = Shell.Current.Navigation.ModalStack;

                        if (shellModalStack.Count > 0)
                        {
                            Debug.WriteLine("Popping modal from Shell...");
                            await Shell.Current.Navigation.PopModalAsync();
                        }
                        else
                        {
                            Debug.WriteLine("No modal to pop from Shell.");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Shell.Current is null. No modal to pop.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error dismissing modal: {ex.Message}");
            }
        }
    }
}
