using BaseballScoringApp.Models;

namespace BaseballScoringApp
{
    public partial class MainPage : ContentPage
    {
        BBDataRepository repo;
        HttpClient _httpClient;
        public MainPage()
        {
            InitializeComponent();
            repo = BBDataRepository.getInstance();
            InitOnLogin();
            _httpClient = new HttpClient();
        }

        private async void InitOnLogin()
        {
            await repo.InitialiseOnLogin();
        }
    }
}
