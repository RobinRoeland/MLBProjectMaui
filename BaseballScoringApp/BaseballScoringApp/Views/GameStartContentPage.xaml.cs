using BaseballModelsLib.Models;
using BaseballScoringApp.Models;
using BaseballScoringApp.ViewModels;
using Microsoft.Maui.Controls.Internals;
using System.Diagnostics;

namespace BaseballScoringApp;

public partial class GameStartContentPage : ContentPage
{
    private readonly BBDataRepository mRepo;
    private GameStartContentPageViewModel mViewModel;

    private BBTeam? selectedHomeTeam;
    private BBTeam? selectedAwayTeam;

    public bool checkForExistingGameOnlyFirstTime = true;
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
        ShowLoadingAnimation();
        await WaitForTeamsToLoad();
        HideLoadingAnimation();

        mViewModel.TeamList = mRepo.mTeamsList; 
        TeamCarrouselHome.ItemsSource = mViewModel.TeamList;
        TeamCarrouselAway.ItemsSource = mViewModel.TeamList;
        
        BBTeam hometeamtostart = mRepo.getRandomTeam();
        BBTeam awayteamtostart = mRepo.getRandomTeam();
        //avoid twice same team
        while (hometeamtostart == awayteamtostart)
        {
            awayteamtostart = mRepo.getRandomTeam();
        }
        mViewModel.SelectedHomeTeam = hometeamtostart;
        mViewModel.SelectedAwayTeam = awayteamtostart;
        if (checkForExistingGameOnlyFirstTime)
        {
            checkForExistingGameOnlyFirstTime = false;

            // check if there is an unfinished game in db for user, if yes, reinitialise and continue
            BBGame lastGame = await mRepo.GetLastRunningGame(Globals.loggedInUser);
            //lastGame = null; 
            if (lastGame != null)
            {
                BBTeam mHomeTeam = mRepo.getTeamByID(lastGame.HomeTeamId);
                BBTeam mAwayTeam = mRepo.getTeamByID(lastGame.AwayTeamId);

                bool answer = await DisplayAlert("Game in progress",
                    $"Game between {mHomeTeam.NameDisplayBrief} vs {mAwayTeam.NameDisplayBrief} in progress. Do you want to restart this game ?",
                    "Yes",
                    "No");
                if (answer)
                {
                    //quick bypass for new game continue the existing game
                    mRepo.mCurrentGame = lastGame;
                    if (mHomeTeam != null && mAwayTeam != null)
                    {
                        mRepo.mCurrentGame.mGameProgress.mGameInProgress = true;
                        //pick 2 pitchers
                        mRepo.mCurrentGame.HomeStartingPitcherId = mHomeTeam.getRandomPlayerPlayingGivenPosition("P").Id;
                        mRepo.mCurrentGame.AwayStartingPitcherId = mAwayTeam.getRandomPlayerPlayingGivenPosition("P").Id;
                        mRepo.mCurrentGame.StartGame(lastGame.TotalInnings);

                        //remove all previous statistics from db table
                        await mRepo.removeAllStatisticsForGameFromRepo(lastGame);
                    }
                }
                else
                {
                    //remove the game and all related scores in db.
                    await mRepo.removeGameFromRepo(lastGame);
                    mRepo.mCurrentGame = null;
                }
            }
            if (mRepo.mCurrentGame != null && mRepo.mCurrentGame.mGameProgress.mGameInProgress)
            {
                // Use the DI container to resolve ScoringContentPage
                var scoringPage = App.Current.Handler.MauiContext.Services.GetService<ScoringContentPage>();
                await Navigation.PushAsync(scoringPage);
                
            }
        }
        //else start game page is ok
    }
    private async Task WaitForTeamsToLoad()
    {
        while (mRepo.isInitialised() == false)
        {
            await Task.Delay(500); // Wait 500ms before checking again
        }
    }

    private void ShowLoadingAnimation()
    {
        WaitingOverlay.IsVisible = true;
     //   StartNewGameScrollView.IsVisible = false;
    }
    private void HideLoadingAnimation()
    {
        WaitingOverlay.IsVisible = false;
       // StartNewGameScrollView.IsVisible = true;
    }
    private async void ButtonStartGame_Clicked(object sender, EventArgs e)
    {
        if (mViewModel.SelectedHomeTeam.Id == mViewModel.SelectedAwayTeam.Id)
        {
            await DisplayAlert("Wrong choice", " You can not play yourself !\nPick another opponent.", "Ok");
            return;
        }

        //continue scoring
        BBGame game = new BBGame();
        game.HomeTeamId = mViewModel.SelectedHomeTeam.Id;
        game.AwayTeamId = mViewModel.SelectedAwayTeam.Id;


        game.HomeStartingPitcherId = mViewModel.SelectedPitcherHomeTeam.Id;
        game.AwayStartingPitcherId= mViewModel.SelectedPitcherAwayTeam.Id;
        game.GameDate = mViewModel.GameDate.ToShortDateString();
        game.GameTime = mViewModel.GameDate.ToShortTimeString();
        game.TotalInnings = mViewModel.NumInningsInGame;
        game.User = Globals.loggedInUser;
        //game.in
        await mRepo.StartANewGame(game);
        if (mRepo.mCurrentGame != null)
        {
            //success
            mRepo.mCurrentGame.mGameProgress.mGameInProgress = true;
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

            mViewModel.SelectedPitcherHomeTeam = selectedHomeTeam.getRandomPitcher();
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

            mViewModel.SelectedPitcherAwayTeam = selectedAwayTeam.getRandomPitcher();
            //Set pitcher list 
            //Set PitcherList from selection for starting pitcher selection
            //Console.WriteLine($"Current item changed to: {newItem}");
        }
    }
}