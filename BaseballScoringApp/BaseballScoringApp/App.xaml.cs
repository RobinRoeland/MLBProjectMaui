using System.Diagnostics;
using BaseballScoringApp.Services;
using MetroLog.Maui;
using Microsoft.Extensions.Logging;

namespace BaseballScoringApp
{
    public partial class App : Application
    {
        ILogger<App> _logger;

        public App(ILogger<App> plogger)
        {
            InitializeComponent();

            _logger = plogger;
            //outputs to Output window MSVC
            _logger.LogInformation($"BaseballScoring app started at {DateTime.Now.ToString()}");
            Globals.logger = _logger;

            // Set a default MainPage immediately
            MainPage = new AppShell();

            // Check if a valid token exists at app startup
            CheckTokenValidity();
        }
        private async void CheckTokenValidity()
        {
            try
            {            // Retrieve the token stored in SecureStorage
                var token = await JwtService.GetTokenAsync();

                if (string.IsNullOrEmpty(token) || !JwtService.IsTokenValid(token))
                {
                    // If token is invalid or expired, show the login page
                    MainPage = new NavigationPage(new MainPageLogin());
                }
                else
                {
                    // If token is valid, show the main page (AppShell)
                    MainPage = new AppShell();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in CheckTokenValidity: {ex.Message}");
                // Ensure we always have a MainPage
                MainPage = new NavigationPage(new MainPageLogin());
            }
        }
    }
}