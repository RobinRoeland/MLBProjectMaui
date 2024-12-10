using BaseballScoringApp.Models;
using BaseballScoringApp.ViewModels;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using Plugin.Maui.Audio;
using CommunityToolkit.Maui.Views;

namespace BaseballScoringApp;

public partial class ScoringContentPage : ContentPage
{
    private Point catcherPosition; // Position of the catcher
    private Point tappedPosition;  // Position of the tapped point
    private bool shouldDrawLine = false;

    private readonly Dictionary<Button, (double X, double Y)> _relativePositions;

    private ScoringContentPageViewModel mViewModel;

    private SoundManager mSoundManager;

    private string currentshownImageBatter; // to avoid reload each refresh.
    private string currentshownImagePitcher; // to avoid reload each refresh.

    public ScoringContentPage(IAudioManager audioManager)
    {
        // de audioManager komt automatisch via dependency injection (zie mauiprogram.cs)

        InitializeComponent();
        mSoundManager = SoundManager.getInstance(); 
        currentshownImageBatter = "";
        currentshownImagePitcher = "";
        
        // Define the relative positions (X and Y as percentages of the image)
        _relativePositions = new Dictionary<Button, (double X, double Y)>();
        setButtonRelativePositionsOfFieldPlayers();

        mViewModel = (ScoringContentPageViewModel)BindingContext;

        // Set the catcher position based on the button's layout
        catcherPosition = new Point(0.5 * Width, 0.9 * Height); // Adjust this dynamically later
        DrawingView.Drawable = new LineDrawable(() => catcherPosition, () => tappedPosition, () => shouldDrawLine, () => BaseballFieldImage);

    }
    public void setButtonRelativePositionsOfFieldPlayers()
    {
        _relativePositions[CatcherButton] = (0.38, 0.9);       // Catcher: Bottom center
        _relativePositions[PitcherButton] = (0.43, 0.58);       // Pitcher: Middle center
        _relativePositions[FirstBaseButton] = (0.60, 0.46);    // 1B: Right-middle
        _relativePositions[SecondBaseButton] = (0.45, 0.30);    // 2B: Top-center
        _relativePositions[ThirdBaseButton] = (0.15, 0.46);    // 3B: Left-middle
        _relativePositions[ShortstopButton] = (0.25, 0.37);    // SS: Between 2B and 3B
        _relativePositions[LeftFieldButton] = (0.06, 0.26);     // LF: Far-left top
        _relativePositions[CenterFieldButton] = (0.38, 0.05);   // CF: Top-center
        _relativePositions[RightFieldButton] = (0.72, 0.26);     // RF: Far-right top

        _relativePositions[BatterButton] = (0.33, 0.82);        // Batter
        _relativePositions[Base1thButton] = (0.55, 0.55);        // Runner 1B
        _relativePositions[Base2ndButton] = (0.35, 0.38);         // Runner 2B
        _relativePositions[Base3rdButton] = (0.2, 0.55);          // Runner 3B
        _relativePositions[BatterIntermediateButton] = (0.44, 0.68);  // runner intermediate button

    }
    private void OnPageAppearing(object sender, EventArgs e)
    {
        setButtonRelativePositionsOfFieldPlayers();
        mViewModel.Game.mGameProgress.mCurrentScoringMode = gameScoringMode.InFieldPlay;
        UpdateScreenData();
    }
    private void OnPlayerButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            DisplayAlert("Player Position", $"You clicked: {button.Text}", "OK");
        }
    }
    private void OnTapGestureRecognizerTapped(object sender, EventArgs args)
    {
        // Handle the tap
        DisplayAlert("titl", $"Tap detected at {((TappedEventArgs)args).Parameter}", "OK");
    }

    // Recalculate button positions when the image size changes
    private void OnImageSizeChanged(object sender, EventArgs e)
    {
        if (BaseballFieldImage.Width <= 0 || BaseballFieldImage.Height <= 0)
            return;

        // Get the actual dimensions of the image
        double imageWidth = BaseballFieldImage.Width;
        double imageHeight = BaseballFieldImage.Height;
        double imageBottomY = BaseballFieldImage.Y;
        double imageLeftX = BaseballFieldImage.X;
        // Reposition all buttons relative to the image
        foreach (var kvp in _relativePositions)
        {
            //tekent met 0,0 boven links buttons en images
            Button button = kvp.Key;
            (double relativeX, double relativeY) = kvp.Value;
            double BtnHalfHeight = button.Height / 2.0;
            double BtnHalfWidth = button.Width / 2.0;

            // Calculate the absolute position for the button
            double absoluteX = imageLeftX + (relativeX * imageWidth) - BtnHalfWidth;
            double absoluteY = imageBottomY + (relativeY * imageHeight) - BtnHalfHeight;

            // Update the button's position in the AbsoluteLayout
            ButtonContainer.SetLayoutBounds(button, new Rect(absoluteX, absoluteY, 40, 40)); // 40x40 is the button size
            ButtonContainer.SetLayoutFlags(button, AbsoluteLayoutFlags.None);
        }
    }
    // Handle Page Tapped
    private void OnPageTapped(object sender, TappedEventArgs e)
    {
        // Record the tapped position
       /* if (e is TappedEventArgs args && Width > 0 && Height > 0)
        {
            Point? position = e.GetPosition((View)sender);
            if (position.HasValue) // Ensure the Point? has a value
            {
                var catcherBounds = CatcherButton.Bounds;
                // Create a Point object with the position
                catcherPosition = new Point(catcherBounds.Center.X, catcherBounds.Center.Y);
                //catcherPosition = new Point(0.5 * Width, 0.9 * Height); // Adjust this dynamically later
                tappedPosition = position.Value; // Safely get the Point value

                // Trigger drawing of the line
                shouldDrawLine = true;
                DrawingView.Invalidate(); // Redraw the GraphicsView
            }
        }*/
    }


    private async Task HandleGameStatus_AtEndOfCurrentPlay()
    {
        // this function is called at the end of execution of all actions, usually after buttonclick
        //purpose is to update screen content and evaluate moving to next inning or end of game
        UpdateScreenData();
        // verify end of inning or ball 4 scenario's

        BBGame currentgame = mViewModel.mRepo.mCurrentGame;
        // handle 3 OUTS situation
        InningStatus inningstate = currentgame.mGameProgress.Handle_OutsInInning();
        // check here if the inning is over, or if a batter change is up
        if (inningstate  == InningStatus.EndOfInning)
        {
            currentgame.mGameProgress.FinishCurrentInning(); // publish scores to api
            currentgame.mGameProgress.Start_NextInning();
        }
        else if (inningstate == InningStatus.EndOfBallGame)
        {
            currentgame.BallGame();
                
            //end screen here
            Navigation.PopAsync();
            // Use the DI container to resolve ScoringContentPage
            var ballgamePage = new EndBallGameContentPage();
            await Navigation.PushAsync(ballgamePage);
        }

        //message display system in model dialog pane that blocks user input 
        while (currentgame.hasMessagesAvailable())
        {
            string msgToShow = currentgame.popMessage();
            PopupDialog_ShowBoard infoboard = new PopupDialog_ShowBoard(msgToShow, 3000);
            await infoboard.ShowAsync(this);          
        }

        UpdateScreenData();

    }
    public void UpdateScreenData()
    {
        if (mViewModel.mRepo == null || mViewModel.mRepo.mCurrentGame == null)
            return;
        BBGameProgress gpr = mViewModel.mRepo.mCurrentGame.mGameProgress;
        BBTeamGameStatus defendingTeam = gpr.getDefendingTeam();
        BBTeamGameStatus offensiveTeam = gpr.getOffensiveTeam();
        if (defendingTeam == null || offensiveTeam == null)
            return;
        UpdateButtonColorsByTeam();

        BBPlayer currentBatter = offensiveTeam.getCurrentBatter();
        PlayerAtBat.Text = $"{currentBatter.Rugnummer.ToString()} {currentBatter.Name}";
        BBPlayer currentPitcher = defendingTeam.mCurrentlyPitching;
        PitcherThrowing.Text = $"{currentPitcher.Rugnummer.ToString()} {currentPitcher.Name}";
        // setplayer images    
        // setting imagebatterurl checks if url is reachable , if not replaces by default local img
        string batterimage = $"https://img.mlbstatic.com/mlb-photos/image/upload/d_people:generic:headshot:silo:current.png/r_max/w_180,q_auto:best/v1/people/{currentBatter.MLBPersonId}/headshot/silo/current";
        if(batterimage != currentshownImageBatter)
        {
            currentshownImageBatter = batterimage;
            SetValidatedImageUrl(batterimage, BatterImage);
        }
        // setting imagepitcherurl checks if url is reachable , if not replaces by default local img
        string pitcherimage = $"https://img.mlbstatic.com/mlb-photos/image/upload/d_people:generic:headshot:silo:current.png/r_max/w_180,q_auto:best/v1/people/{currentPitcher.MLBPersonId}/headshot/silo/current";
        if (pitcherimage != currentshownImagePitcher)
        {
            currentshownImagePitcher = pitcherimage;
            SetValidatedImageUrl(pitcherimage, PitcherImage);
        }

        //update players
        FirstBaseButton.Text = defendingTeam.mTeam.getPlayerTypeFromLineUp("1B").Rugnummer.ToString();
        SecondBaseButton.Text = defendingTeam.mTeam.getPlayerTypeFromLineUp("2B").Rugnummer.ToString();
        ThirdBaseButton.Text = defendingTeam.mTeam.getPlayerTypeFromLineUp("3B").Rugnummer.ToString();

        ShortstopButton.Text = defendingTeam.mTeam.getPlayerTypeFromLineUp("SS").Rugnummer.ToString();

        LeftFieldButton.Text = defendingTeam.mTeam.getPlayerTypeFromLineUp("LF").Rugnummer.ToString();
        CenterFieldButton.Text = defendingTeam.mTeam.getPlayerTypeFromLineUp("CF").Rugnummer.ToString();
        RightFieldButton.Text = defendingTeam.mTeam.getPlayerTypeFromLineUp("RF").Rugnummer.ToString();

        PitcherButton.Text = defendingTeam.mCurrentlyPitching.Rugnummer.ToString();
        CatcherButton.Text = defendingTeam.mTeam.getPlayerTypeFromLineUp("C").Rugnummer.ToString();

        //update scoreboard
        Pitcher_Home.Text = $"{gpr.mHomeTeam.mCurrentlyPitching.Rugnummer:D2}";
        Pitcher_Away.Text = $"{gpr.mAwayTeam.mCurrentlyPitching.Rugnummer:D2}";
        Team_Home.Text = gpr.mHomeTeam.mTeam.NameDisplayBrief;
        Team_Away.Text = gpr.mAwayTeam.mTeam.NameDisplayBrief;
        Balls.Text = $"{gpr.mBalls:D1}";
        Strikes.Text = $"{gpr.mStrikes:D1}";
        Outs.Text = $"{mViewModel.Game.mGameProgress.mOuts:D1}";

        R_Home.Text = $"{gpr.mHomeTeam.Runs:D}";
        R_Away.Text = $"{gpr.mAwayTeam.Runs:D}";
        H_Home.Text = $"{gpr.mHomeTeam.Hits:D}";
        H_Away.Text = $"{gpr.mAwayTeam.Hits:D}";
        E_Home.Text = $"{gpr.mHomeTeam.Errors:D}";
        E_Away.Text = $"{gpr.mAwayTeam.Errors:D}";

        UpdateInningScores();

        //Update Inning 
        InningCurrent.Text = $"{gpr.mCurrentInning}";
        if(gpr.mCurrentSideInning == InningSide.Top)
        {
            InningTopImg.IsVisible = true ;
            InningBottomImg.IsVisible = false;
        }
        else
        {
            InningBottomImg.IsVisible = true;
            InningTopImg.IsVisible = false;
        }

        //batter name
        BatterButton.Text = offensiveTeam.getCurrentBatter().Rugnummer.ToString();
        //Fielder buttons
        if (gpr.mRunnerOn1thBase == null)
        {
            Base1thButton.IsVisible = false;
            Base1thButton.Text = "";
        }
        else
        {
            Base1thButton.IsVisible = true;
            Base1thButton.Text = gpr.mRunnerOn1thBase.Rugnummer.ToString();
        }
            
        if (gpr.mRunnerOn2ndBase== null)
        {
            Base2ndButton.Text = "";
            Base2ndButton.IsVisible = false;
        }            
        else
        {
            Base2ndButton.IsVisible = true;
            Base2ndButton.Text = gpr.mRunnerOn2ndBase.Rugnummer.ToString();
        }
        if (gpr.mRunnerOn3rdBase == null)
        {
            Base3rdButton.Text = "";
            Base3rdButton.IsVisible = false;
        }
        else
        {
            Base3rdButton.IsVisible = true;
            Base3rdButton.Text = gpr.mRunnerOn3rdBase.Rugnummer.ToString();
        }

        // set buttons bottom of screen depending on field play
        if (gpr.mCurrentScoringMode == gameScoringMode.Pitching)
        {
            //pitching mode
            PitchingPlay_ButtonGroup.IsVisible = true;
            InfieldPlay_ButtonGroup.IsVisible = false;
            BatterIntermediateButton.IsVisible = false;
            BatterButton.IsVisible = true;
        }
        else
        {
            //in field play
            PitchingPlay_ButtonGroup.IsVisible = false;
            InfieldPlay_ButtonGroup.IsVisible = true;
            // if infield play and the action of moving the batter over is still active, show intermediate button
            if(gpr.mRunnerOnIntermediateWaitPosition != null)
            {
                BatterIntermediateButton.IsVisible = true;
                BatterButton.IsVisible = false;
                BatterIntermediateButton.Text = gpr.mRunnerOnIntermediateWaitPosition.Rugnummer.ToString();
            }
            else
            {
                BatterButton.IsVisible = true;
                BatterIntermediateButton.IsVisible = false;
            }
            
        }      
    }

    public void UpdateInningScores()
    {
        BBGameProgress gpr = mViewModel.mRepo.mCurrentGame.mGameProgress;
        I_Home_1.Text = $"{gpr.mHomeTeam.getInningScore(1):D}";
        I_Away_1.Text = $"{gpr.mAwayTeam.getInningScore(1):D}";
        if (mViewModel.Game.TotalInnings >= 2)
        {
            Inning2_Layout.IsVisible = true;
            I_Home_2.Text = $"{gpr.mHomeTeam.getInningScore(2):D}";
            I_Away_2.Text = $"{gpr.mAwayTeam.getInningScore(2):D}";
        }
        else
            Inning2_Layout.IsVisible = false;
        if (mViewModel.Game.TotalInnings >= 3)
        {
            Inning3_Layout.IsVisible = true;
            I_Home_3.Text = $"{gpr.mHomeTeam.getInningScore(3):D}";
            I_Away_3.Text = $"{gpr.mAwayTeam.getInningScore(3):D}";
        }
        else
            Inning3_Layout.IsVisible = false;
        if (mViewModel.Game.TotalInnings >= 4)
        {
            Inning4_Layout.IsVisible = true;
            I_Home_4.Text = $"{gpr.mHomeTeam.getInningScore(4):D}";
            I_Away_4.Text = $"{gpr.mAwayTeam.getInningScore(4):D}";
        }
        else
            Inning4_Layout.IsVisible = false;
        if (mViewModel.Game.TotalInnings >= 5)
        {
            Inning5_Layout.IsVisible = true;
            I_Home_5.Text = $"{gpr.mHomeTeam.getInningScore(5):D}";
            I_Away_5.Text = $"{gpr.mAwayTeam.getInningScore(5):D}";
        }
        else
            Inning5_Layout.IsVisible = false;
        if (mViewModel.Game.TotalInnings >= 6)
        {
            Inning6_Layout.IsVisible = true;
            I_Away_6.Text = $"{gpr.mAwayTeam.getInningScore(6):D}";
            I_Home_6.Text = $"{gpr.mHomeTeam.getInningScore(6):D}";
        }
        else
            Inning6_Layout.IsVisible = false;
        if (mViewModel.Game.TotalInnings >= 7)
        {
            Inning7_Layout.IsVisible = true;
            I_Away_7.Text = $"{gpr.mAwayTeam.getInningScore(7):D}";
            I_Home_7.Text = $"{gpr.mHomeTeam.getInningScore(7):D}";
        }
        else
            Inning7_Layout.IsVisible = false;
        if (mViewModel.Game.TotalInnings >=8)
        {
            Inning8_Layout.IsVisible = true;
            I_Home_8.Text = $"{gpr.mHomeTeam.getInningScore(8):D}";
            I_Away_8.Text = $"{gpr.mAwayTeam.getInningScore(8):D}";
        }
        else
            Inning8_Layout.IsVisible = false;
        Inning9_Layout.IsVisible = false;
        if (mViewModel.Game.TotalInnings == 9)
        {
            Inning9_Layout.IsVisible = true;
            I_Home_9.Text = $"{gpr.mHomeTeam.getInningScore(9):D}";
            I_Away_9.Text = $"{gpr.mAwayTeam.getInningScore(9):D}";
        }
        else
            Inning9_Layout.IsVisible = false;
    }
    public void UpdateButtonColorsByTeam()
    {
        BBGameProgress gpr = mViewModel.mRepo.mCurrentGame.mGameProgress;
        BBTeamGameStatus defendingTeam = gpr.getDefendingTeam();
        BBTeamGameStatus offensiveTeam = gpr.getOffensiveTeam();
        CatcherButton.BackgroundColor = defendingTeam.mTeam.mTeamColor;
        PitcherButton.BackgroundColor = defendingTeam.mTeam.mTeamColor;
        FirstBaseButton.BackgroundColor = defendingTeam.mTeam.mTeamColor;
        SecondBaseButton.BackgroundColor = defendingTeam.mTeam.mTeamColor;
        ThirdBaseButton.BackgroundColor = defendingTeam.mTeam.mTeamColor;
        ShortstopButton.BackgroundColor = defendingTeam.mTeam.mTeamColor;
        LeftFieldButton.BackgroundColor = defendingTeam.mTeam.mTeamColor;
        CenterFieldButton.BackgroundColor = defendingTeam.mTeam.mTeamColor;
        RightFieldButton.BackgroundColor = defendingTeam.mTeam.mTeamColor;
        Base1thButton.BackgroundColor = offensiveTeam.mTeam.mTeamColor;
        Base2ndButton.BackgroundColor = offensiveTeam.mTeam.mTeamColor;
        Base3rdButton.BackgroundColor = offensiveTeam.mTeam.mTeamColor;
        BatterButton.BackgroundColor = offensiveTeam.mTeam.mTeamColor;
        BatterIntermediateButton.BackgroundColor = offensiveTeam.mTeam.mTeamColor;
    }

    private async Task<string> ValidateImageUrl(string url)
    {
        if (!await IsImageReachable(url))
        {
            return "defaultplayerimg.png"; // Path to a local image if the URL is invalid
        }
        return url;
    }
    private async Task SetValidatedImageUrl(string url, Image imageview)
    {
        url = await ValidateImageUrl(url);
        imageview.Source = url;
    }

    private async Task<bool> IsImageReachable(string url)
    {
        //Checks if a given URL can be reached (used to check if img is present online)
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    private async void ButtonBall_Clicked(object sender, EventArgs e)
    {
        mViewModel.Game.DoAction(new GameAction_Ball(mViewModel.Game.mGameProgress.getCurrentBatter()));
        HandleGameStatus_AtEndOfCurrentPlay();
    }
    private void ButtonStrike_Clicked(object sender, EventArgs e)
    {
        mViewModel.Game.DoAction(new GameAction_Strike(mViewModel.Game.mGameProgress.getCurrentBatter()));
        HandleGameStatus_AtEndOfCurrentPlay();
    }

    private void ButtonFoul_Clicked(object sender, EventArgs e)
    {
        mViewModel.Game.DoAction(new GameAction_FoulBall(mViewModel.Game.mGameProgress.getCurrentBatter()));
        //mSoundManager.PlaySound("mp3/heisout.mp3");
        HandleGameStatus_AtEndOfCurrentPlay();
    }

    
    private async void OnPlayerBase1Clicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            // user click on 1st base handle game options
            List<IGameAction> possibleActionList = mViewModel.Game.mGameProgress.getPossibleGameActions(FieldPositions.runnerOn1st);
            if (possibleActionList.Count > 0)
            {
                await handleChoiceFromActionList(possibleActionList);
            }
            else
                await DisplayAlert("", "No possible actions for " + mViewModel.Game.mGameProgress.mRunnerOn1thBase.Name, "Ok");
        }
    }

    private async void OnPlayerBase2Clicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            // user click on 2nd base handle game options
            List<IGameAction> possibleActionList = mViewModel.Game.mGameProgress.getPossibleGameActions(FieldPositions.runnerOn2nd);
            if(possibleActionList.Count > 0)
            {
                await handleChoiceFromActionList(possibleActionList);
            }
            else
                await DisplayAlert("", "No possible actions for " + mViewModel.Game.mGameProgress.mRunnerOn2ndBase.Name, "Ok");
        }
    }
    private async void OnPlayerBase3Clicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            // user click on 3rd base handle game options
            List<IGameAction> possibleActionList = mViewModel.Game.mGameProgress.getPossibleGameActions(FieldPositions.runnerOn3rd);
            if (possibleActionList.Count > 0)
            {
                await handleChoiceFromActionList(possibleActionList);
            }
            else
                await DisplayAlert("", "No possible actions for " + mViewModel.Game.mGameProgress.mRunnerOn3rdBase.Name, "Ok");
        }
    }
    private async Task<IGameAction> handleChoiceFromActionList(List<IGameAction> possibleActionList)
    {
        //generic function to select an action from a list of actions
        var popup = new DynamicButtonActionList(possibleActionList);
        IGameAction chosenAction = await this.ShowPopupAsync(popup) as IGameAction;

        // Handle the selected action
        if (chosenAction != null)
        {
            // execute the chosen action
            chosenAction.DoAction(mViewModel.Game);

            HandleGameStatus_AtEndOfCurrentPlay();
            
            //await DisplayAlert("Selected Command", chosenAction.ActionDisplayName, "OK");
        }
        return chosenAction;
    }
    private async void OnPlayerBatterClicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            // user click on 2nd base handle game options
            List<IGameAction> possibleActionList = mViewModel.Game.mGameProgress.getPossibleGameActions(FieldPositions.batterbox);
            if (possibleActionList.Count > 0)
            {
                await handleChoiceFromActionList(possibleActionList);
            }
            else
                await DisplayAlert("", "No possible actions for " + mViewModel.Game.mGameProgress.getCurrentBatter().Name, "Ok");
        }
    }
    private async void ButtonEndOfPlay_Clicked(object sender, EventArgs e)
    {
        // end of in field play mode -> go back to pitching mode if can
        if (sender is Button button)
        {
            string s = "tbd";
            mViewModel.Game.mGameProgress.mCurrentScoringMode = gameScoringMode.Pitching;
            HandleGameStatus_AtEndOfCurrentPlay();
        }
    }


}

// Custom Drawable for the GraphicsView
public class LineDrawable : IDrawable
{
    private readonly Func<Point> getCatcherPosition;
    private readonly Func<Point> getTappedPosition;
    private readonly Func<bool> getShouldDrawLine;
    private readonly Func<Image> getFieldImage;

    public LineDrawable(Func<Point> getCatcherPosition, Func<Point> getTappedPosition, Func<bool> getShouldDrawLine, Func<Image> fieldimage)
    {
        this.getCatcherPosition = getCatcherPosition;
        this.getTappedPosition = getTappedPosition;
        this.getShouldDrawLine = getShouldDrawLine;
        this.getFieldImage = fieldimage;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (getShouldDrawLine())
        {
            canvas.StrokeColor = Colors.Red;
            canvas.StrokeSize = 2;

            var catcher = getCatcherPosition();
            var tapped = getTappedPosition();
            Image fieldimage = getFieldImage();

            canvas.DrawLine((float)catcher.X, (float)catcher.Y, (float)tapped.X, (float)tapped.Y);
            canvas.DrawLine((float)0, (float)0, (float)fieldimage.X + (float)fieldimage.Width, (float)fieldimage.Y + (float)fieldimage.Height);
        }
    }

}