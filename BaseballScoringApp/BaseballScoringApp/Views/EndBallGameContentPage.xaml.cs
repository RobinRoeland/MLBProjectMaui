using BaseballScoringApp.Models;

namespace BaseballScoringApp;

public partial class EndBallGameContentPage : ContentPage
{
    private BBGame mGame;
	public EndBallGameContentPage()
	{
		InitializeComponent();
        mGame = BBDataRepository.getInstance().mCurrentGame;
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Set Home Team Data
        HomeTeamNameLabel.Text = mGame.mGameProgress.mHomeTeam.mTeam.Name;
        HomeTeamScoreLabel.Text = mGame.mGameProgress.mHomeTeam.Runs.ToString(); 
        HomeTeamLogo.Source =  $"{mGame.mGameProgress.mHomeTeam.mTeam.FranchiseCode.ToLower()}.png";

        // Set Away Team Data
        AwayTeamNameLabel.Text = mGame.mGameProgress.mAwayTeam.mTeam.Name; ;
        AwayTeamScoreLabel.Text = mGame.mGameProgress.mAwayTeam.Runs.ToString(); 
        AwayTeamLogo.Source = $"{mGame.mGameProgress.mAwayTeam.mTeam.FranchiseCode.ToLower()}.png";
    }

    private async void OnEndGameButtonClicked(object sender, EventArgs e)
    {
        // Navigate back to GameStartContentPage
        if (Navigation.NavigationStack.Count > 0)
        {
            await Navigation.PopToRootAsync(); // Navigate back to the root page in the stack
        }
    }
}