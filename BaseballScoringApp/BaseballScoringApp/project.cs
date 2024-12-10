using BaseballScoringApp.Models;

namespace BaseballScoringApp;

public partial class GameScoringNotLoggedOn : ContentPage
{
	public GameScoringNotLoggedOn()
	{
		InitializeComponent();


    }

    private void ContentPage_Appearing(object sender, EventArgs e)
    {
        if (BBDataRepository.getInstance().mLoggedIn)
        {
            Navigation.PopAsync();
            Navigation.PushAsync(new GameStartContentPage());
        }
        // Activated when entering tab from navigation, constructor only happens once on startup
        BBDataRepository.getInstance().mLoggedIn = !BBDataRepository.getInstance().mLoggedIn;
    }
}