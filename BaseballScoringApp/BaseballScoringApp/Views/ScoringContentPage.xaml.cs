using BaseballScoringApp.Models;
using BaseballScoringApp.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using Plugin.Maui.Audio;
using CommunityToolkit.Maui.Views;
using System.Diagnostics;
namespace BaseballScoringApp;

public partial class ScoringContentPage : ContentPage
{
    private Point? mLastTappedPosition;  // Position of the tapped point
    
    private readonly Dictionary<Frame, (double X, double Y)> _relativePositions;

    private ScoringContentPageViewModel mViewModel;

    private SoundManager mSoundManager;

    private string currentshownImageBatter; // to avoid reload each refresh.
    private string currentshownImagePitcher; // to avoid reload each refresh.

    private List<ImageButton> mDefensiveButtonsSelected; // only active with infield play
    private LinesDrawable mDrawingLinesOnGraphicsView;
    private Color mDefensivePlayColor;

    List<string> insults;

    public ScoringContentPage(IAudioManager audioManager)
    {
        // de audioManager komt automatisch via dependency injection (zie mauiprogram.cs)

        InitializeComponent();
        mSoundManager = SoundManager.getInstance(); 
        currentshownImageBatter = "";
        currentshownImagePitcher = "";
        mDefensivePlayColor = Colors.Red;
        mLastTappedPosition = null;

        // Define the relative positions (X and Y as percentages of the image)
        _relativePositions = new Dictionary<Frame, (double X, double Y)>();
        setButtonRelativePositionsOfFieldPlayers();

        mViewModel = (ScoringContentPageViewModel)BindingContext;

        // Set the catcher position based on the button's layout
        mDefensiveButtonsSelected = new List<ImageButton>();
        mDrawingLinesOnGraphicsView = new LinesDrawable();

        var gestureRecognizer = new TapGestureRecognizer
        {
            NumberOfTapsRequired = 2 // Set to 2 for double-click/double-tap
        };
        gestureRecognizer.Tapped += OnDefenderButton_DoubleTapped;

        // capture doubleclick on button of defenders
        CatcherButton.GestureRecognizers.Add(gestureRecognizer);
        PitcherButton.GestureRecognizers.Add(gestureRecognizer);
        FirstBaseButton.GestureRecognizers.Add(gestureRecognizer);
        SecondBaseButton.GestureRecognizers.Add(gestureRecognizer);
        ThirdBaseButton.GestureRecognizers.Add(gestureRecognizer);
        ShortstopButton.GestureRecognizers.Add(gestureRecognizer);
        LeftFieldButton.GestureRecognizers.Add(gestureRecognizer);
        CenterFieldButton.GestureRecognizers.Add(gestureRecognizer);
        RightFieldButton.GestureRecognizers.Add(gestureRecognizer);

        insults = new List<string>();
        insults.Add("mp3/blue.mp3");
        insults.Add("mp3/bluecount.mp3");
        insults.Add("mp3/bluegreatgame.mp3");
        insults.Add("mp3/bluebeer.mp3");
        insults.Add("mp3/blueconsist.mp3");
    }
    public void setButtonRelativePositionsOfFieldPlayers()
    {
        _relativePositions[CatcherBtnFrame] = (0.36, 0.9);       // Catcher: Bottom center
        _relativePositions[PitcherBtnFrame] = (0.36, 0.57);       // Pitcher: Middle center
        _relativePositions[FirstBaseBtnFrame] = (0.60, 0.46);    // 1B: Right-middle
        _relativePositions[SecondBaseBtnFrame] = (0.45, 0.30);    // 2B: Top-center
        _relativePositions[ThirdBaseBtnFrame] = (0.10, 0.46);    // 3B: Left-middle
        _relativePositions[ShortstopBtnFrame] = (0.23, 0.32);    // SS: Between 2B and 3B
        _relativePositions[LeftFieldBtnFrame] = (0.06, 0.26);     // LF: Far-left top
        _relativePositions[CenterFieldBtnFrame] = (0.38, 0.05);   // CF: Top-center
        _relativePositions[RightFieldBtnFrame] = (0.72, 0.26);     // RF: Far-right top
        _relativePositions[BatterBtnFrame] = (0.28, 0.80);        // Batter
        _relativePositions[Base1thBtnFrame] = (0.55, 0.55);        // Runner 1B
        _relativePositions[Base2ndBtnFrame] = (0.35, 0.38);         // Runner 2B
        _relativePositions[Base3rdBtnFrame] = (0.2, 0.55);          // Runner 3B
        _relativePositions[BatterIntermediateBtnFrame] = (0.44, 0.68);  // runner intermediate button
    }
    private void OnPageAppearing(object sender, EventArgs e)
    {
        setButtonRelativePositionsOfFieldPlayers();
        HandleGameStatus_AtEndOfCurrentPlay();// UpdateScreenData();
    }
    private async void OnPlayerButtonClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button)
        {
            if (mViewModel.Game.mGameProgress.mCurrentScoringMode == gameScoringMode.InFieldPlay)
            {
                clickDefensiveButtonInfieldPlay(button);
                HandleGameStatus_AtEndOfCurrentPlay();// UpdateScreenData();
            }
        }
    }
    private async void OnDefenderButton_DoubleTapped(object sender, EventArgs e)
    {
        //await DisplayAlert("Double Click", "Button was double-clicked", "OK");
        if (sender is ImageButton button)
        {
            List<IGameAction> possibleActionList = null;
            // add catcher options in pitching mode (passedball)
            if (sender == CatcherButton && mViewModel.Game.mGameProgress.mCurrentScoringMode == gameScoringMode.Pitching)
            {
                possibleActionList = mViewModel.Game.mGameProgress.getPossibleGameActions(FieldPositions.catcher);
            }
            else if (sender == PitcherButton && mViewModel.Game.mGameProgress.mCurrentScoringMode == gameScoringMode.Pitching)
            {
                possibleActionList = mViewModel.Game.mGameProgress.getPossibleGameActions(FieldPositions.pitcher);
            }
            // add all player options in n field play mode (error)
            else if (mViewModel.Game.mGameProgress.mCurrentScoringMode == gameScoringMode.InFieldPlay)
            {
                FieldPositions position  = FieldPositions.batterbox;
                if(sender==CatcherButton) position = FieldPositions.catcher; 
                else if (sender == PitcherButton) position = FieldPositions.pitcher;
                else if (sender == FirstBaseButton) position = FieldPositions.firstbase;
                else if (sender == SecondBaseButton) position = FieldPositions.secondbase;
                else if (sender == ThirdBaseButton) position = FieldPositions.thirdbase;
                else if (sender == ShortstopButton) position = FieldPositions.shortstop;
                else if (sender == LeftFieldButton) position = FieldPositions.leftfield;
                else if (sender == CenterFieldButton) position = FieldPositions.centerfield;
                else if (sender == RightFieldButton) position = FieldPositions.rightfield;
                if(position != FieldPositions.batterbox)//if a position is set
                    possibleActionList = mViewModel.Game.mGameProgress.getPossibleGameActions(position);             
            }
            if (possibleActionList != null && possibleActionList.Count > 0)
            {
                await handleChoiceFromActionList(possibleActionList);
            }
        }

    }
    private void OnTapGestureRecognizerTapped(object sender, TappedEventArgs e)
    {
        // Handle the tap only if infieldplay modus
        if (mViewModel.Game.mGameProgress.mCurrentScoringMode == gameScoringMode.InFieldPlay)
        {
            
            if (e is not null && e.GetPosition(DrawingView) is Point tapPosition && Width > 0 && Height > 0)
            {             
            // Record the tapped position
                Point? position = e.GetPosition((View)sender);
                if (position.HasValue) // Ensure the Point? has a value
                {
                    mLastTappedPosition = position;
                    HandleGameStatus_AtEndOfCurrentPlay();// UpdateScreenData();
                }
            }
        }
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
        Frame buttonframe;
        double relativeX, relativeY;
        double BtnHalfHeight, BtnHalfWidth;
        double absoluteX, absoluteY;
        // Reposition all buttons relative to the image
        foreach (var kvp in _relativePositions)
        {
            //tekent met 0,0 boven links buttons en images
            buttonframe = kvp.Key;
            (relativeX, relativeY) = kvp.Value;
            BtnHalfHeight = buttonframe.Height / 2.0;
            BtnHalfWidth = buttonframe.Width / 2.0;
            // Calculate the absolute position for the button
            absoluteX = imageLeftX + (relativeX * imageWidth) - BtnHalfWidth;
            absoluteY = imageBottomY + (relativeY * imageHeight) - BtnHalfHeight;

            // Update the button's position in the AbsoluteLayout
            ButtonContainer.SetLayoutBounds(buttonframe, new Rect(absoluteX, absoluteY, 50, 45)); // 40x40 is the button size
            ButtonContainer.SetLayoutFlags(buttonframe, AbsoluteLayoutFlags.None);
        }
        if(mViewModel.Game.mGameProgress.mCurrentScoringMode == gameScoringMode.InFieldPlay)
            drawDefensiveplay();

    }
    // Handle Page Tapped
    private void OnPageTapped(object sender, TappedEventArgs e)
    {
        //on windows version comes here, on android hits in ontapgesturerecognised function
        if(mViewModel.Game.mGameProgress.mCurrentScoringMode == gameScoringMode.InFieldPlay)
        {
            // Record the tapped position
            if (e is TappedEventArgs args && Width > 0 && Height > 0)
            {
                Point? position = e.GetPosition((View)sender);
                if (position.HasValue) // Ensure the Point? has a value
                {
                    Point updatedPosition = new Point(position.Value.X, position.Value.Y - ScoreBoardGrid.Height);
                    mLastTappedPosition = updatedPosition;
                    HandleGameStatus_AtEndOfCurrentPlay(); // UpdateScreenData();
                }
            }
        }
    }

    private async Task HandleGameStatus_AtEndOfCurrentPlay()
    {
        // this function is called at the end of execution of all actions, usually after buttonclick
        //purpose is to update screen content and evaluate moving to next inning or end of game
        UpdateScreenData();
        // verify end of inning or ball 4 scenario's

        BBGame currentgame = mViewModel.mRepo.mCurrentGame;
        InningStatus inningstate = InningStatus.InProgress;
        // handle 3 OUTS situation
        // check here if the inning is over, or if a batter change is up
        if (currentgame.mGameProgress.mCurrentScoringMode == gameScoringMode.Pitching)
        {
            // inning can only be over in pitching mode, not while in field play mode,
            // //   (eg batter can hit out but still need to register other positions till end of play is clicked)
            inningstate = currentgame.mGameProgress.Handle_OutsInInning();
            if (inningstate == InningStatus.EndOfInning)
            {
                currentgame.mGameProgress.FinishCurrentInning(); // publish scores to api
                currentgame.mGameProgress.Start_NextInning();
            }
            else if (inningstate == InningStatus.EndOfBallGame)
            {
                // jump to score view is at end of function, now set game complete and publish last scores
                SoundManager sm = SoundManager.getInstance();
                sm.PlaySound("mp3/ballgame.mp3");
                currentgame.BallGame(BBDataRepository.getInstance());
            }
        }
        //message display system in model dialog pane that blocks user input and shows message queue till empty
        while (currentgame.hasMessagesAvailable())
        {
            string msgToShow = currentgame.popMessage();
            PopupDialog_ShowBoard infoboard = new PopupDialog_ShowBoard(msgToShow, 3000);
            await infoboard.ShowAsync(this);          
        }

        if(currentgame.mGameProgress.getOffensiveTeam().mCurrentBatter == null && !currentgame.Finished)
            currentgame.mGameProgress.getOffensiveTeam().MoveToNextBatterInLineUp();

        UpdateScreenData();

        // after all messages are shown and endofballgame state, change screen
        if (inningstate == InningStatus.EndOfBallGame && currentgame.mGameProgress.mGameCompleted)
        {
            //End scorescreen here
            // Use the DI container to resolve ScoringContentPage
            var ballgamePage = App.Current.Handler.MauiContext.Services.GetService<EndBallGameContentPage>();
            await Navigation.PushAsync(ballgamePage);
        }

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

        BBPlayer currentBatter = offensiveTeam.getCurrentBatter();
        BBPlayer currentPitcher = defendingTeam.mCurrentlyPitching;

        // if a runner is still on intermediate position and bases have becomes free, move him over
        if (gpr.mCurrentScoringMode == gameScoringMode.InFieldPlay)
        { 
            if(gpr.mRunnerOnIntermediateWaitPosition != null)
                gpr.tryToMoveIntermediateRunnerToDestination();
        }

        // now draw screen
        UpdateButtonColorsByTeam();

        if (currentBatter != null)
        {
            int batterinlineupseq = gpr.getOffensiveTeam().mCurrentBatterNumber;
            PlayerAtBat.Text = $"{currentBatter.Rugnummer.ToString()} {currentBatter.Name} ({batterinlineupseq})";
            string batterimage = currentBatter.getImageUrl();
            if (batterimage != currentshownImageBatter)
            {
                currentshownImageBatter = batterimage;
                SetValidatedImageUrl(currentBatter, BatterImage);
            }
        }
        else
        {
            PlayerAtBat.Text = $"New Batter Up";
            BatterImage.Source = "defaultplayerimg.png";
        }
        int pitches = gpr.getDefendingTeam().TotalPitches;
        PitcherThrowing.Text = $"{currentPitcher.Rugnummer.ToString()} {currentPitcher.Name} ({pitches})";
        PitchCount.Text = $"{defendingTeam.TotalPitches:D3}";
        // setplayer images    
        // setting imagebatterurl checks if url is reachable , if not replaces by default local img
        // setting imagepitcherurl checks if url is reachable , if not replaces by default local img
        string pitcherimage = currentPitcher.getImageUrl();
        if (pitcherimage != currentshownImagePitcher)
        {
            currentshownImagePitcher = pitcherimage;
            SetValidatedImageUrl(currentPitcher, PitcherImage);
        }

        // Now update the button image source to the player's image URL
        //update players
        BBPlayer DefFirstBase = defendingTeam.mTeam.getPlayerTypeFromLineUp("1B");
        SetValidatedImageUrl(DefFirstBase, FirstBaseButton);
        FirstBaseButtonTxt.Text = DefFirstBase.Rugnummer.ToString();

        BBPlayer DefSecondBase = defendingTeam.mTeam.getPlayerTypeFromLineUp("2B");
        SetValidatedImageUrl(DefSecondBase, SecondBaseButton);
        SecondBaseButtonTxt.Text = DefSecondBase.Rugnummer.ToString();

        BBPlayer DefThirdBase = defendingTeam.mTeam.getPlayerTypeFromLineUp("3B");
        SetValidatedImageUrl(DefThirdBase, ThirdBaseButton);
        ThirdBaseButtonTxt.Text = DefThirdBase.Rugnummer.ToString();

        BBPlayer DefShortStop= defendingTeam.mTeam.getPlayerTypeFromLineUp("SS");
        SetValidatedImageUrl(DefShortStop, ShortstopButton);
        ShortstopButtonTxt.Text = DefShortStop.Rugnummer.ToString();

        BBPlayer DefLeftField = defendingTeam.mTeam.getPlayerTypeFromLineUp("LF");
        SetValidatedImageUrl(DefLeftField, LeftFieldButton);
        LeftFieldButtonTxt.Text = DefLeftField.Rugnummer.ToString();
        
        BBPlayer DefCenterField= defendingTeam.mTeam.getPlayerTypeFromLineUp("CF");
        SetValidatedImageUrl(DefCenterField, CenterFieldButton);
        CenterFieldButtonTxt.Text = DefCenterField.Rugnummer.ToString();

        BBPlayer DefRightFields= defendingTeam.mTeam.getPlayerTypeFromLineUp("RF");
        SetValidatedImageUrl(DefRightFields, RightFieldButton);
        RightFieldButtonTxt.Text = DefRightFields.Rugnummer.ToString();

        SetValidatedImageUrl(currentPitcher, PitcherButton);
        PitcherButtonTxt.Text = defendingTeam.mCurrentlyPitching.Rugnummer.ToString();

        BBPlayer catcher = defendingTeam.mTeam.getPlayerTypeFromLineUp("C");
        SetValidatedImageUrl(catcher, CatcherButton);
        CatcherButtonTxt.Text = catcher.Rugnummer.ToString();

        //update scoreboard
        Pitcher_Home.Text = $"{gpr.mHomeTeam.mCurrentlyPitching.Rugnummer:D2}";
        Pitcher_Away.Text = $"{gpr.mAwayTeam.mCurrentlyPitching.Rugnummer:D2}";
        Team_Home.Text = gpr.mHomeTeam.mTeam.NameDisplayBrief;
        Team_Away.Text = gpr.mAwayTeam.mTeam.NameDisplayBrief;
        Balls.Text = $"{gpr.mBalls:D1}";
        Strikes.Text = $"{gpr.mStrikesForOutCounting:D1}";
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
        if (currentBatter != null)
        {
            BatterBtnFrame.IsVisible = true;
            SetValidatedImageUrl(currentBatter, BatterButton);
            BatterButtonTxt.Text = currentBatter.Rugnummer.ToString();
        }
        else
        {
            BatterBtnFrame.IsVisible = false;
            BatterButton.Source = "defaultplayerimg.png";
            BatterButtonTxt.Text = "Next Batter Up";
        }
        //Fielder buttons
        if (gpr.mRunnerOn1thBase == null)
        {
            Base1thBtnFrame.IsVisible = false;
            Base1thButtonTxt.Text = "";
        }
        else
        {
            Base1thBtnFrame.IsVisible = true;
            SetValidatedImageUrl(gpr.mRunnerOn1thBase, Base1thButton);
            Base1thButtonTxt.Text = gpr.mRunnerOn1thBase.Rugnummer.ToString();
        }
            
        if (gpr.mRunnerOn2ndBase== null)
        {
            Base2ndButtonTxt.Text = "";
            Base2ndBtnFrame.IsVisible = false;
        }            
        else
        {
            Base2ndBtnFrame.IsVisible = true;
            SetValidatedImageUrl(gpr.mRunnerOn2ndBase, Base2ndButton);
            Base2ndButtonTxt.Text = gpr.mRunnerOn2ndBase.Rugnummer.ToString();
        }
        if (gpr.mRunnerOn3rdBase == null)
        {
            Base3rdButtonTxt.Text = "";
            Base3rdBtnFrame.IsVisible = false;
        }
        else
        {
            Base3rdBtnFrame.IsVisible = true;
            SetValidatedImageUrl(gpr.mRunnerOn3rdBase, Base3rdButton);
            Base3rdButtonTxt.Text = gpr.mRunnerOn3rdBase.Rugnummer.ToString();
        }

        // set buttons bottom of screen depending on field play
        if (gpr.mCurrentScoringMode == gameScoringMode.Pitching)
        {
            //pitching mode
            PitchingPlay_ButtonGroup.IsVisible = true;
            InfieldPlay_ButtonGroup.IsVisible = false;
            BatterIntermediateBtnFrame.IsVisible = false;
            BatterBtnFrame.IsVisible = true;
        }
        else
        {
            //in field play mode
            PitchingPlay_ButtonGroup.IsVisible = false;
            InfieldPlay_ButtonGroup.IsVisible = true;
            BatterBtnFrame.IsVisible = false;
            // if infield play and the action of moving the batter over is still active, show intermediate button
            if (gpr.mRunnerOnIntermediateWaitPosition != null)
            {
                BatterIntermediateBtnFrame.IsVisible = true;
                SetValidatedImageUrl(gpr.mRunnerOnIntermediateWaitPosition, BatterIntermediateButton);
                BatterIntermediateButtonTxt.Text = gpr.mRunnerOnIntermediateWaitPosition.Rugnummer.ToString();
            }
            else
            {
                BatterIntermediateBtnFrame.IsVisible = false;
            }

            //draw defenseplay
            drawDefensiveplay();
        }      
    }

    public void drawDefensiveplay()
    {
        if (mLastTappedPosition != null)
        {
            // if there is a hit position, draw the hitline
            Point pCat = GetAbsolutePosition(CatcherButton);
            Point pBat = GetAbsolutePosition(BatterButton);
            pCat.Y -= ScoreBoardGrid.Height - (CatcherButton.Height / 2.0);
            pBat.Y -= ScoreBoardGrid.Height - (BatterButton.Height / 2.0);

            Point originPosition = new Point(pCat.X + (CatcherButton.Width / 2.0), pBat.Y);

            // Create a Point object with the position
            //catcherPosition = new Point(0.5 * Width, 0.9 * Height); // Adjust this dynamically later
            DrawableLine hitline = new DrawableLine(originPosition, mLastTappedPosition.Value, mViewModel.Game.mGameProgress.getOffensiveTeam().mTeam.mTeamColor, 2, new float[] { 1, 1 });
            mDrawingLinesOnGraphicsView.mHitLine = hitline;
        }
        
        mDrawingLinesOnGraphicsView.ClearDefensiveList();
        if (mDefensiveButtonsSelected.Count < 2)
        {
            mDrawingLinesOnGraphicsView.ClearDefensiveList();
            DrawingView.Invalidate();
        }
        else
        {
            for (int i = 0; i < mDefensiveButtonsSelected.Count - 1; i++)
            {
                ImageButton b1 = mDefensiveButtonsSelected[i];
                ImageButton b2 = mDefensiveButtonsSelected[i + 1];
                var lb1 = GetAbsolutePosition(b1);
                var lb2 = GetAbsolutePosition(b2);
                // remove height of scoreboard as graphics draw with 0,0 equal to bottom left corner of scoreboard
                // add half button height and width to center
                lb1.Y -= ScoreBoardGrid.Height - (b1.Height / 2.0);
                lb2.Y -= ScoreBoardGrid.Height - (b2.Height / 2.0);
                lb1.X += (b1.Width / 2.0);
                lb2.X += (b2.Width / 2.0);
                DrawableLine line = new DrawableLine(lb1, lb2, mDefensivePlayColor, 3, new float[] { 6, 3 });// Dashed pattern: 10 pixels line, 5 pixels space
                mDrawingLinesOnGraphicsView.Lines.Add(line);
            }
        }
        // Trigger drawing of the lines
        DrawingView.Drawable = mDrawingLinesOnGraphicsView;
        DrawingView.Invalidate();
    }
    public static Point GetAbsolutePosition(VisualElement element)
    {
        double x = element.X;
        double y = element.Y;
        Element parent = element.Parent;

        while (parent is VisualElement visualParent)
        {
            x += visualParent.X;
            y += visualParent.Y;
            parent = visualParent.Parent;
        }

        return new Point(x, y);
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
        CatcherBtnFrame.BorderColor = defendingTeam.mTeam.mTeamColor;
        PitcherBtnFrame.BorderColor = defendingTeam.mTeam.mTeamColor;
        FirstBaseBtnFrame.BorderColor = defendingTeam.mTeam.mTeamColor;
        SecondBaseBtnFrame.BorderColor = defendingTeam.mTeam.mTeamColor;
        ThirdBaseBtnFrame.BorderColor = defendingTeam.mTeam.mTeamColor;
        ShortstopBtnFrame.BorderColor = defendingTeam.mTeam.mTeamColor;
        LeftFieldBtnFrame.BorderColor = defendingTeam.mTeam.mTeamColor;
        RightFieldBtnFrame.BorderColor = defendingTeam.mTeam.mTeamColor;
        CenterFieldBtnFrame.BorderColor = defendingTeam.mTeam.mTeamColor;

        CatcherButtonTxt.BackgroundColor = defendingTeam.mTeam.mTeamColor;
        PitcherButtonTxt.BackgroundColor = defendingTeam.mTeam.mTeamColor;
        FirstBaseButtonTxt.BackgroundColor = defendingTeam.mTeam.mTeamColor;
        SecondBaseButtonTxt.BackgroundColor = defendingTeam.mTeam.mTeamColor;
        ThirdBaseButtonTxt.BackgroundColor = defendingTeam.mTeam.mTeamColor;
        ShortstopButtonTxt.BackgroundColor = defendingTeam.mTeam.mTeamColor;
        LeftFieldButtonTxt.BackgroundColor = defendingTeam.mTeam.mTeamColor;
        CenterFieldButtonTxt.BackgroundColor = defendingTeam.mTeam.mTeamColor;
        RightFieldButtonTxt.BackgroundColor = defendingTeam.mTeam.mTeamColor;
        
        Base1thButtonTxt.BackgroundColor = offensiveTeam.mTeam.mTeamColor;
        Base2ndButtonTxt.BackgroundColor = offensiveTeam.mTeam.mTeamColor;
        Base3rdButtonTxt.BackgroundColor = offensiveTeam.mTeam.mTeamColor;
        BatterButtonTxt.BackgroundColor = offensiveTeam.mTeam.mTeamColor;
        BatterIntermediateButtonTxt.BackgroundColor = offensiveTeam.mTeam.mTeamColor;
        Base1thBtnFrame.BorderColor= offensiveTeam.mTeam.mTeamColor;
        Base2ndBtnFrame.BorderColor = offensiveTeam.mTeam.mTeamColor;
        Base3rdBtnFrame.BorderColor = offensiveTeam.mTeam.mTeamColor;
        BatterBtnFrame.BorderColor = offensiveTeam.mTeam.mTeamColor;
        BatterIntermediateBtnFrame.BorderColor = offensiveTeam.mTeam.mTeamColor;

        // draw defensive play in inplay mode
        if(gpr.mCurrentScoringMode == gameScoringMode.InFieldPlay)
        {
            for (int i = 0; i < mDefensiveButtonsSelected.Count ; i++)
            {
                ImageButton btn = mDefensiveButtonsSelected[i];
                Frame fr = (Frame)btn.Parent.Parent;
                fr.BorderColor = mDefensivePlayColor;
            }
                
        }
        

    }

    private async Task<ImageSource> ValidateImageUrl(string url)
    {
        //Checks if a given URL can be reached (used to check if img is present online)
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(url);
        }
        catch
        {
            return ImageSource.FromFile("defaultplayerimg.png");  // Path to a local image if the URL is invalid
        }
        return ImageSource.FromUri(new Uri(url));
    }
    private async Task SetValidatedImageUrl(BBPlayer player, Image imageview)
    {
        if(player.mPlayerImage != null)
        {
            imageview.Source = player.mPlayerImage;
        }
        else
        {
            //load from url and cache in player
            string url = player.getImageUrl();
            player.mPlayerImage = await ValidateImageUrl(url);
            imageview.Source = player.mPlayerImage;
        }
    }
    private async Task SetValidatedImageUrl(BBPlayer player, ImageButton imageButton)
    {
        if(player.mPlayerImage == null || player.mPlayerImage.IsEmpty)
        {
            //load from url and cache in player
            string url = player.getImageUrl();
            player.mPlayerImage = await ValidateImageUrl(url);
            imageButton.Source = player.mPlayerImage;
        }
        if (player.mPlayerImage != null && !player.mPlayerImage.IsEmpty)
        {
            //if (imageButton.Source != player.mPlayerImage)
            //{
                //only refresh if differnet
                imageButton.Source = player.mPlayerImage;
            //}
        }
        else
        {
            //no image
            imageButton.Source = ImageSource.FromFile("defaultplayerimg.png");
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
        if (sender is ImageButton button)
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
        if (sender is ImageButton button)
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
        if (sender is ImageButton button)
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
        if (sender is ImageButton button)
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
    private async void ButtonInsult_Clicked(object sender, EventArgs e)
    {
        var random = new Random();
        SoundManager sm = SoundManager.getInstance();
        sm.PlaySound(insults[random.Next(insults.Count)]);
    }
    private async void ButtonEndOfPlay_Clicked(object sender, EventArgs e)
    {
        // end of in field play mode -> go back to pitching mode if can
        if (sender is Button button)
        {
            if(mViewModel.Game.mGameProgress.mRunnerOnIntermediateWaitPosition != null)
            {
                // cannot end the infield play as long as runner is on intermediate pos between home and first base
                // something was not scored
                string dest = "";
                switch(mViewModel.Game.mGameProgress.mInfieldPlayAction.mHitAndRunToBase)
                {
                    case FieldPositions.firstbase: dest = "first base"; break;
                    case FieldPositions.secondbase: dest = "second base"; break;
                    case FieldPositions.thirdbase: dest = "third base"; break;
                };
                DisplayAlert("In Field Play", "Batter is still on intermediate position going to "+dest+".\nwhat happened on the bases ?", "Ok");
            }
            else
            {
                // end the infield play mode
                mDefensiveButtonsSelected.Clear();
                mDrawingLinesOnGraphicsView.ClearDefensiveList();//to erase defense lines
                mLastTappedPosition = null;// to erase hitline
                mDrawingLinesOnGraphicsView.mHitLine = null;
                DrawingView.Invalidate();
                mViewModel.Game.mGameProgress.EndInfieldPlay();

                //end of infield play is always move to next batter
                //jur out mViewModel.Game.mGameProgress.getOffensiveTeam().MoveToNextBatterInLineUp();
                mViewModel.Game.mGameProgress.getOffensiveTeam().mCurrentBatter = null;

                HandleGameStatus_AtEndOfCurrentPlay();
            }
        }
    }
    public void clickDefensiveButtonInfieldPlay(ImageButton btn)
    {
        //acts as toggle:
        if(mDefensiveButtonsSelected.Contains(btn) == false)
            mDefensiveButtonsSelected.Add(btn);
        else
            mDefensiveButtonsSelected.Remove(btn);

        // set last known defensive play in gameprogress
        mViewModel.Game.mGameProgress.mLastKnownDefensivePlay = "";
        for (int i = 0; i < mDefensiveButtonsSelected.Count; i++)
        {   
            ImageButton imgbtn = mDefensiveButtonsSelected[i];
            mViewModel.Game.mGameProgress.mLastKnownDefensivePlay += translateButtonToPositionNumber(imgbtn);
            if (i < mDefensiveButtonsSelected.Count - 1)
                mViewModel.Game.mGameProgress.mLastKnownDefensivePlay += "-";
        }
    }


    public string translateButtonToPositionNumber(ImageButton btn)
    {
        string returnv = "0";
        if (btn == PitcherButton) 
            returnv = "1";
        if (btn == CatcherButton)
            returnv = "2";
        if (btn == FirstBaseButton)
            returnv = "3"; 
        if (btn == SecondBaseButton)
            returnv = "4"; 
        if (btn == ThirdBaseButton) 
            returnv = "5"; 
        if (btn == ShortstopButton) 
            returnv = "6"; 
        if (btn == LeftFieldButton)
            returnv = "7"; 
        if (btn == CenterFieldButton) 
            returnv = "8"; 
        if (btn == RightFieldButton) 
            returnv = "9"; 
        return returnv;
    }
}
public class DrawableLine
{
    public Point Start { get; set; }
    public Point End { get; set; }
    public Color LineColor { get; set; } = Colors.Black;
    public float LineThickness { get; set; } = 1.0f;
    public float[]? DashPattern { get; set; } // Optional dash pattern

    public DrawableLine(Point start, Point end, Color color, float thickness, float[]? dashPattern = null)
    {
        Start = start;
        End = end;
        LineColor = color;
        LineThickness = thickness;
        DashPattern = dashPattern;
    }
}

public class LinesDrawable : IDrawable
{
    //collection of all lines to draw on canvas
    public List<DrawableLine> Lines { get; set; } = new List<DrawableLine>();
    public DrawableLine? mHitLine = null;
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // ball hit line
        if(mHitLine != null)
        {
            canvas.StrokeColor = mHitLine.LineColor;
            canvas.StrokeSize = mHitLine.LineThickness;
            if (mHitLine.DashPattern != null)
            {
                canvas.StrokeDashPattern = mHitLine.DashPattern; // Set the dash pattern
            }
            else
            {
                canvas.StrokeDashPattern = null; // Reset to solid line
            }
            canvas.DrawLine((float)mHitLine.Start.X, (float)mHitLine.Start.Y, (float)mHitLine.End.X, (float)mHitLine.End.Y);
        }
        //now defense lines
        foreach (var line in Lines)
        {
            canvas.StrokeColor = line.LineColor;
            canvas.StrokeSize = line.LineThickness;

            if (line.DashPattern != null)
            {
                canvas.StrokeDashPattern = line.DashPattern; // Set the dash pattern
            }
            else
            {
                canvas.StrokeDashPattern = null; // Reset to solid line
            }
            canvas.DrawLine((float)line.Start.X, (float)line.Start.Y, (float)line.End.X, (float)line.End.Y);
        }
    }
    public void ClearDefensiveList()
    {
        Lines.Clear();
    }
}