using BaseballModelsLib.Models;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public enum InningSide { Top, Bottom };
    public enum gameScoringMode { Pitching, InFieldPlay };
    public class BBGameProgress
    {
        public BBGame mParent;

        public bool mGameInProgress;
        public bool mGameCompleted;

        public int mCurrentInning;
        public InningSide mCurrentSideInning;

        public BBTeamGameStatus? mHomeTeam;
        public BBTeamGameStatus? mAwayTeam;

        public int mBalls;
        public int mStrikes;
        public int mFouls;
        public int mOuts;

        public int mBattersInInning;

        public int mBallsInInning;
        public int mStrikesInInning;
        public int mFoulsInInning;

        public BBPlayer? mRunnerOn1thBase;
        public BBPlayer? mRunnerOn2ndBase;
        public BBPlayer? mRunnerOn3rdBase;
        
        // if game mode is infield play, and bases are not free yet, batter goes to intermediate pos
        public BBPlayer? mRunnerOnIntermediateWaitPosition; 

        public Stack<string> mMessagesToDisplayStack;

        public ScoreManager mScoreManager; // collects all scores during inning half and publishes to server at end of inning

        public gameScoringMode mCurrentScoringMode; // used to track if pictching or infieldplay scoring
        public BBGameProgress(BBGame parent)
        {
            mParent = parent;
            mHomeTeam = null;
            mAwayTeam = null;
            mGameInProgress = false;
            mGameCompleted = true;
            mRunnerOn1thBase = null;
            mRunnerOn2ndBase = null;
            mRunnerOn3rdBase = null;
            mRunnerOnIntermediateWaitPosition = null;
            mBallsInInning = 0;
            mStrikesInInning = 0;
            mFoulsInInning = 0;
            mBattersInInning = 0;
            mMessagesToDisplayStack = new Stack<string>();
            mScoreManager = new ScoreManager();
            mScoreManager.setGameContext(parent);

            mCurrentScoringMode = gameScoringMode.Pitching;
        }

        public async void InitialiseNewGame(BBTeam home, BBTeam away, int NumInningToPlay)
        {
            mHomeTeam = new BBTeamGameStatus(home, this);
            mAwayTeam = new BBTeamGameStatus(away, this);
            mCurrentInning = 1;
            mCurrentSideInning = InningSide.Top;
            if (NumInningToPlay > 9)
                NumInningToPlay = 9;
            else if (NumInningToPlay < 1)
                NumInningToPlay = 1;
            mParent.TotalInnings = NumInningToPlay;
            mGameInProgress = true;
            mGameCompleted = false;
            mRunnerOn1thBase = null;
            mRunnerOn2ndBase = null;
            mRunnerOn3rdBase = null;
            mRunnerOnIntermediateWaitPosition = null;
            mBalls = 0;
            mStrikes = 0;
            mOuts = 0;
            mFouls = 0;
            mBattersInInning = 1; // inning always starts at one as batter already selected
            //generate a lineup
            home.GenerateLineUp_ForGame();
            away.GenerateLineUp_ForGame();

            mHomeTeam.startGame();
            mAwayTeam.startGame();
        }
        public void Handle_3Strikes_BatterOut()
        {
            mOuts++;
            restartThePitchCount();

            //next batter from the lineup
            BBTeamGameStatus offseniveteam = getOffensiveTeam();
            offseniveteam.MoveToNextBatterInLineUp();

            addMessage("Batter Out !");
        }
        public void Start_NextInning()
        {
            // moves to the next side inning of the game and completes the game when finished
            // resets runners at each turn
            if (mCurrentSideInning == InningSide.Top)
            {
                mCurrentSideInning = InningSide.Bottom;
            }
            else
            {
                if (mCurrentInning < mParent.TotalInnings)
                {
                    mCurrentInning++;
                    mCurrentSideInning = InningSide.Top;
                }
                else
                    mGameCompleted = true;
            }
            // clear all scores from previous inning
            mScoreManager.clearAllScores(); ;

            // reset inning data
            mRunnerOn1thBase = null;
            mRunnerOn2ndBase = null;
            mRunnerOn3rdBase = null;
            mStrikesInInning = mBallsInInning = mFoulsInInning = 0;
            mOuts = 0;
            mBalls = mStrikes = mFouls = 0;
            mBattersInInning = 1; //always start at one as new batter already selected

        }
        public void restartThePitchCount()
        {
            // add to totals of inning and then reset
            mBallsInInning += mBalls;
            mStrikesInInning += mStrikes;
            mFoulsInInning += mFoulsInInning;

            mBalls = 0;
            mStrikes = 0;
            mFouls = 0;

        }
        public async void FinishCurrentInning()
        {
            // add totals balls/strikes/fouls for pitcher
            BBPlayer currentpitcher = getCurrentPitcher();
            mScoreManager.registerScore("BaseOnBalls", currentpitcher, mBallsInInning);
            mScoreManager.registerScore("Strikes", currentpitcher, mStrikesInInning);
            mScoreManager.registerScore("Fouls", currentpitcher, mFoulsInInning);
            mScoreManager.registerScore("BattersFaced", currentpitcher, mBattersInInning);
            // then send to api
            await mScoreManager.publishScoreList();

        }
        public BBPlayer getCurrentPitcher()
        {
            return getDefendingTeam().mCurrentlyPitching;
        }
        public BBPlayer getCurrentBatter()
        {
            BBPlayer player = null;
            // The home team always starts in the field, meaning their pitcher takes the mound.
            // //The away team starts batting.
            if (mCurrentSideInning == InningSide.Top)
                player = mAwayTeam.getCurrentBatter();
            else
                player = mHomeTeam.getCurrentBatter();
            return player;
        }
        public BBTeamGameStatus getDefendingTeam()
        {
            if (mCurrentSideInning == InningSide.Top)
                return mHomeTeam;
            return mAwayTeam;
        }
        public BBTeamGameStatus getOffensiveTeam()
        {
            if (mCurrentSideInning == InningSide.Top)
                return mAwayTeam;
            return mHomeTeam;
        }
        public InningStatus Handle_OutsInInning()
        {
            //function returns the state of the inning depending on the current outs
            //check outs
            if (mOuts == 3)
            {
                if (mCurrentInning == mParent.TotalInnings && mCurrentSideInning == InningSide.Bottom)
                {
                    addMessage("Ballgame !");
                    return InningStatus.EndOfBallGame;
                }
                else
                {
                    addMessage("Change Field !");
                    return InningStatus.EndOfInning;
                }
            }
            return InningStatus.InProgress;
        }
        public void BallGame()
        {
            mGameCompleted = true;

            //add games played stats for all in lineup
            foreach (BBPlayer homeplayer in mHomeTeam.mTeam.mLineUpList)
            {
                mScoreManager.registerScore("GamesPlayed", homeplayer, 1);
            }
            mScoreManager.registerScore("GamesPlayed", mHomeTeam.mCurrentlyPitching, 1);
            foreach (BBPlayer awayplayer in mAwayTeam.mTeam.mLineUpList)
            {
                mScoreManager.registerScore("GamesPlayed", awayplayer, 1);
            }
            mScoreManager.registerScore("GamesPlayed", mAwayTeam.mCurrentlyPitching, 1);

            //book in api
            mScoreManager.publishScoreList();
        }
        public void Handle_AdvanceRunnerOn1st()
        {
            if (mRunnerOn2ndBase != null)
            {
                Handle_AdvanceRunnerOn2nd();
            }
            mRunnerOn2ndBase = mRunnerOn1thBase;
            mRunnerOn1thBase = null;

        }
        public void Handle_AdvanceRunnerOn2nd()
        {
            if (mRunnerOn3rdBase != null)
            {
                Handle_AdvanceRunnerOn3rd();
            }
            mRunnerOn3rdBase = mRunnerOn2ndBase;
            mRunnerOn2ndBase = null;

        }
        public void Handle_AdvanceRunnerOn3rd()
        {
            if (mRunnerOn3rdBase != null)
            {
                // a run scores
                BBTeamGameStatus offsensiveteam = getOffensiveTeam();
                offsensiveteam.addRunScoreToInning(mCurrentInning);
                addMessage($"{mRunnerOn3rdBase.Name} ({mRunnerOn3rdBase.Rugnummer.ToString()})\nscores a run.");

                //Add RBI scoring for batter
                mScoreManager.registerScore("RBI", getCurrentBatter(), 1);
                mScoreManager.registerScore("Runs", mRunnerOn3rdBase, 1);
            }
            mRunnerOn3rdBase = null;

        }
        public void addActionsForPlayerBattingPosition(List<IGameAction> actionList)
        {
            // what can happen to the batter
            actionList.Add(new GameAction_WalkBatter(getCurrentBatter(), "HitByPitch"));
            actionList.Add(new GameAction_Hit_InFieldPlay(getCurrentBatter(), FieldPositions.firstbase, "Single"));
            actionList.Add(new GameAction_Hit_InFieldPlay(getCurrentBatter(), FieldPositions.secondbase, "Double"));
            actionList.Add(new GameAction_Hit_InFieldPlay(getCurrentBatter(), FieldPositions.thirdbase, "Triple"));
            actionList.Add(new GameAction_Hit_InFieldPlay(getCurrentBatter(), FieldPositions.homeplate, "Home Run!"));
            actionList.Add(new GameAction_Hit_InFieldPlay(getCurrentBatter(), FieldPositions.homeplate, "Home Run!"));
        }
        public void addActionsForFirstBasePosition(List<IGameAction> actionList)
        {
            if (mCurrentScoringMode == gameScoringMode.Pitching)
            {   //which actions are possible for the runner on first base in pitching mode
                actionList.Add(new GameAction_PickOff(mRunnerOn1thBase, FieldPositions.firstbase, "Pickoff"));
                if (mRunnerOn2ndBase == null)
                {
                    actionList.Add(new GameAction_StealBase(mRunnerOn1thBase, FieldPositions.secondbase, "Steal 2nd base"));
                    actionList.Add(new GameAction_CaughtStealing(mRunnerOn1thBase, FieldPositions.firstbase, "Caught Stealing 2nd"));
                }
            }
            else
            {   //which actions are possible for the runner on first base in infieldplay mode
                actionList.Add(new GameAction_CaughtOutInPlay(mRunnerOn1thBase, "Out On 1st base"));

                if (mRunnerOn2ndBase == null)
                {
                    actionList.Add(new GameAction_MoveToBase(mRunnerOn1thBase, FieldPositions.firstbase, FieldPositions.secondbase, "Slides to 2nd base"));
                    if (mRunnerOn3rdBase == null)
                    { 
                        actionList.Add(new GameAction_MoveToBase(mRunnerOn1thBase, FieldPositions.firstbase, FieldPositions.thirdbase, "Slides to 3rd base"));
                        actionList.Add(new GameAction_MoveToBase(mRunnerOn1thBase, FieldPositions.firstbase, FieldPositions.homeplate, "Scores a run"));
                    }
                }
            }
        }
        public void addActionsFor2ndBasePosition(List<IGameAction> actionList)
        {
            if (mCurrentScoringMode == gameScoringMode.Pitching)
            {   //which actions are possible for the runner on 2nd base in pitching mode
                actionList.Add(new GameAction_PickOff(mRunnerOn2ndBase, FieldPositions.secondbase, "Pickoff"));
                if (mRunnerOn3rdBase == null) // 3 empty
                {
                    actionList.Add(new GameAction_StealBase(mRunnerOn2ndBase, FieldPositions.thirdbase, "Steal 3rd base"));
                    actionList.Add(new GameAction_CaughtStealing(mRunnerOn2ndBase, FieldPositions.secondbase, "Caught Stealing 3rd"));
                }
            }
            else
            {   //which actions are possible for the runner on 2nd base in infieldplay mode
                actionList.Add(new GameAction_CaughtOutInPlay(mRunnerOn2ndBase, "Out On 2nd base"));
                if (mRunnerOn3rdBase == null)
                {
                    actionList.Add(new GameAction_MoveToBase(mRunnerOn2ndBase, FieldPositions.secondbase, FieldPositions.thirdbase, "Slides to 3rd base"));
                    actionList.Add(new GameAction_MoveToBase(mRunnerOn2ndBase, FieldPositions.secondbase, FieldPositions.homeplate, "Scores a run"));
                }
            }
        }
        public void addActionsFor3rdBasePosition(List<IGameAction> actionList)
        {
            if (mCurrentScoringMode == gameScoringMode.Pitching)
            {   //which actions are possible for the runner on 3rd base in pitching mode
                actionList.Add(new GameAction_PickOff(mRunnerOn3rdBase, FieldPositions.thirdbase, "Pickoff"));
                actionList.Add(new GameAction_StealBase(mRunnerOn3rdBase, FieldPositions.homeplate, "Steal home plate"));
                actionList.Add(new GameAction_CaughtStealing(mRunnerOn3rdBase, FieldPositions.thirdbase, "Caught Stealing home"));
            }
            else
            {   //which actions are possible for the runner on 3rd base in infieldplay mode
                actionList.Add(new GameAction_CaughtOutInPlay(mRunnerOn3rdBase, "Out On 3rd base"));
                actionList.Add(new GameAction_MoveToBase(mRunnerOn3rdBase, FieldPositions.thirdbase, FieldPositions.homeplate, "Scores a run"));
           }
        }
        public List<IGameAction> getPossibleGameActions(FieldPositions forPosition)
        {
            List<IGameAction> actionList = new List<IGameAction>();
            if (forPosition == FieldPositions.batterbox)
            {
                addActionsForPlayerBattingPosition(actionList);
            }
            if (forPosition == FieldPositions.runnerOn1st)
            {
                addActionsForFirstBasePosition(actionList);
            }
            else if (forPosition == FieldPositions.runnerOn2nd)
            {
                addActionsFor2ndBasePosition(actionList); 
            }
            else if (forPosition == FieldPositions.runnerOn3rd)
            {
                addActionsFor3rdBasePosition(actionList);
            }
            return actionList;
        }
        public void addMessage(string msgToShow)
        {
            mMessagesToDisplayStack.Push(msgToShow);
        }
        public bool hasMessagesAvailable()
        {
            return (mMessagesToDisplayStack.Count > 0);
        }
        public string popMessage()
        {
            return mMessagesToDisplayStack.Pop();
        }


    }
}
