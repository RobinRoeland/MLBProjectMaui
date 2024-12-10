using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace BaseballScoringApp.Services
{
    internal class JwtService
    {
        // Securely store the JWT token
        public static async Task SaveTokenAsync(string token)
        {
            await SecureStorage.SetAsync("jwt_token", token);
        }

        // Retrieve the stored JWT token
        public static async Task<string> GetTokenAsync()
        {
            return await SecureStorage.GetAsync("jwt_token");
        }

        // Delete the JWT token from storage
        public static async Task DeleteTokenAsync()
        {
            SecureStorage.Remove("jwt_token");
        }

        // Check if the JWT token is valid
        public static bool IsTokenValid(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            try
            {
                var parts = token.Split('.');

                if (parts.Length != 3)
                    return false;

                var payload = parts[1];
                payload = payload.Replace('-', '+').Replace('_', '/');  // Base64 URL decoding
                var jsonBytes = Convert.FromBase64String(payload);
                var json = Encoding.UTF8.GetString(jsonBytes);
                var jObject = JObject.Parse(json);

                // Get expiration time ('exp') from token payload
                var exp = jObject["exp"]?.ToObject<long>();
                if (exp == null)
                    return false;

                var expiryDate = UnixTimeStampToDateTime(exp.Value);
                return expiryDate > DateTime.UtcNow;
            }
            catch (Exception)
            {
                return false; // If there's any issue decoding or parsing the token, it's invalid
            }
        }

        // Convert Unix timestamp to DateTime
        private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            var unixStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return unixStart.AddSeconds(unixTimeStamp).ToLocalTime();
        }

        // Logout by removing the token and redirecting to the login page
        public static async Task LogoutAsync()
        {
            // Remove token from secure storage
            SecureStorage.Remove("jwt_token");

            // Redirect to login page (you can set this in App.xaml.cs as part of your navigation logic)
            Application.Current.MainPage = new NavigationPage(new MainPageLogin());
            // Ensure the login page is a modal
            Device.BeginInvokeOnMainThread(async () =>
            {
                await (Application.Current.MainPage as NavigationPage).Navigation.PushModalAsync(new MainPageLogin());
            });
        }
    }
}
