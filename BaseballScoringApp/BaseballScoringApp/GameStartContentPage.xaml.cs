using BaseballModelsLib.Models;
using BaseballScoringApp.Models;
using BaseballScoringApp.ViewModels;
using Microsoft.Maui.Controls.Internals;

namespace BaseballScoringApp;

public partial class GameStartContentPage : ContentPage
{
    private readonly BBDataRepository mRepo;
    private GameStartContentPageViewModel mViewModel;

    private BBTeam? selectedHomeTeam;
    private BBTeam? selectedAwayTeam;
    public GameStartContentPage()
    {
        mRepo = BBDataRepository.getInstance();
        selectedAwayTeam = null;
        selectedHomeTeam= null;

        InitializeComponent();

        AccessViewModel();
    }
    private async void OnPageAppearing(object sender, EventArgs e)
    {
        BBGame lastGame = await mRepo.GetLastRunningGame();
        //lastGame = null; 
        if (lastGame != null)
        {
            //quick bypass for new game
            mRepo.mCurrentGame = lastGame;
            BBTeam mHomeTeam = mRepo.getTeamByID(lastGame.HomeTeamId);
            BBTeam mAwayTeam = mRepo.getTeamByID(lastGame.AwayTeamId);
            if (mHomeTeam != null && mAwayTeam != null)
            {
                mRepo.mCurrentGame.mGameProgress.mGameInProgress = true;
                //pick 2 pitchers
                mRepo.mCurrentGame.HomeStartingPitcherId = mHomeTeam.getRandomPlayerPlayingGivenPosition("P").Id;
                mRepo.mCurrentGame.AwayStartingPitcherId = mAwayTeam.getRandomPlayerPlayingGivenPosition("P").Id;
                mRepo.mCurrentGame.StartGame(lastGame.TotalInnings);
            }
        }
        /*        if (!mRepo.mLoggedIn)
                {
                    //move to scoring start page
                    Navigation.PopAsync();
                    Navigation.PushAsync(new GameScoringNotLoggedOn());
                }*/
        if (mRepo.mCurrentGame != null && mRepo.mCurrentGame.mGameProgress.mGameInProgress)
        {
            //continue scoring
            Navigation.PopAsync();
            // Use the DI container to resolve ScoringContentPage
            var scoringPage = App.Current.Handler.MauiContext.Services.GetService<ScoringContentPage>();
            await Navigation.PushAsync(scoringPage);
        }
        //else start game page is ok
    }

    private async void ButtonStartGame_Clicked(object sender, EventArgs e)
    {
        //continue scoring
        if(BindingContext is GameStartContentPageViewModel viewModel)
            DisplayAlert("game", mViewModel.SelectedHomeTeam.NameDisplayBrief + " vs " + mViewModel.SelectedAwayTeam.NameDisplayBrief, "OK");

        BBGame game = new BBGame();
        game.HomeTeamId = mViewModel.SelectedHomeTeam.Id;
        game.AwayTeamId = mViewModel.SelectedAwayTeam.Id;
        game.HomeStartingPitcherId = mViewModel.SelectedPitcherHomeTeam.Id;
        game.AwayStartingPitcherId= mViewModel.SelectedPitcherAwayTeam.Id;
        game.GameDate = mViewModel.GameDate.ToShortDateString();
        game.GameTime = mViewModel.GameDate.ToShortTimeString();
        game.TotalInnings = mViewModel.NumInningsInGame;
        //game.in
        await mRepo.StartANewGame(game);
        if (mRepo.mCurrentGame != null)
        {
            //success
            mRepo.mCurrentGame.mGameProgress.mGameInProgress = true;
            await Navigation.PopAsync();
            // Use the DI container to resolve ScoringContentPage
            var scoringPage = App.Current.Handler.MauiContext.Services.GetService<ScoringContentPage>();
            await Navigation.PushAsync(scoringPage);
        }
        else
            DisplayAlert("Error", "Game creation failed on cloud, try again.", "Ok");
    }

    private void AccessViewModel()
    {
        mViewModel = null;
        // Cast the BindingContext to the ViewModel type
        if (BindingContext is GameStartContentPageViewModel viewModel)
        {
            mViewModel = (GameStartContentPageViewModel)BindingContext;
            // Access properties or call methods on the ViewModel
        }
    }

    private void CarouselViewHome_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {
        // Get the new current item
        var newItem = e.CurrentItem as string;

        if (e.CurrentItem == null)
            return;

        BBTeam? selectedTeam = e.CurrentItem as BBTeam;

        if (selectedTeam != null && selectedTeam != selectedHomeTeam && selectedTeam.FranchiseCode != null)
        {
            selectedHomeTeam = selectedTeam;

            mViewModel.PitchersForSelectedHomeTeam = selectedHomeTeam.mPitchers;
            HomeLogo.Source = $"{selectedHomeTeam.FranchiseCode.ToLower()}.png";
            //Set pitcher list 
            //Set PitcherList from selection for starting pitcher selection
            //Console.WriteLine($"Current item changed to: {newItem}");
        }
    }

    private void CarouselViewAway_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {
        // Get the new current item
        if (e.CurrentItem == null)
            return;

        BBTeam? selectedTeam = e.CurrentItem as BBTeam;

        if (selectedTeam != null && selectedTeam != selectedAwayTeam && selectedTeam.FranchiseCode != null)
        {
            selectedAwayTeam = selectedTeam;

            mViewModel.PitchersForSelectedAwayTeam= selectedAwayTeam.mPitchers;
            AwayLogo.Source = $"{selectedAwayTeam.FranchiseCode.ToLower()}.png";

            //Set pitcher list 
            //Set PitcherList from selection for starting pitcher selection
            //Console.WriteLine($"Current item changed to: {newItem}");
        }
    }
}