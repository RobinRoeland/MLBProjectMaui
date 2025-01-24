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
        public int mBaseOnBallsInInning;

        public int mStrikesForOutCounting;
        public int mBattersInInning;

        public int mBallsInInning;
        public int mStrikesInInning;
        public int mFoulsInInning;

        public BBPlayer? mRunnerOn1thBase;
        public BBPlayer? mRunnerOn2ndBase;
        public BBPlayer? mRunnerOn3rdBase;
        
        // if game mode is infield play, and bases are not free yet, batter goes to intermediate pos
        public BBPlayer? mRunnerOnIntermediateWaitPosition;
        //is filled when in mode infield play, it contains the action of the batter
        public GameAction_Hit_InFieldPlay? mInfieldPlayAction;


        public Stack<string> mMessagesToDisplayStack;

        public int mTotalPitchCount;
        public ScoreManager mScoreManager; // collects all scores during inning half and publishes to server at end of inning

        public gameScoringMode mCurrentScoringMode; // used to track if pictching or infieldplay scoring
        
        public string mLastKnownDefensivePlay;// constantly updated from view : eg : "9-5-1"
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
            mInfieldPlayAction = null;
            mBallsInInning = 0;
            mStrikesInInning = 0;
            mStrikesForOutCounting = 0;
            mFoulsInInning = 0;
            mBattersInInning = 0;
            mBaseOnBallsInInning = 0;
            mMessagesToDisplayStack = new Stack<string>();
            mScoreManager = new ScoreManager();
            mScoreManager.setGameContext(parent);
            mLastKnownDefensivePlay = "";
            mTotalPitchCount= 0;
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
            mInfieldPlayAction = null; 
            mBalls = 0;
            mStrikes = 0;
            mOuts = 0;
            mFouls = 0;
            mStrikesForOutCounting = 0;
            mBaseOnBallsInInning = 0;
            mTotalPitchCount = 0;
            mBattersInInning = 0; 
            mLastKnownDefensivePlay = "";
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
            // jur out offseniveteam.MoveToNextBatterInLineUp();
            //this line will force picking a new batter in game 
            offseniveteam.mCurrentBatter = null;

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
            mBattersInInning = 0;
            mStrikesForOutCounting = 0;
            mBaseOnBallsInInning = 0; // in inning
        }
        public void restartThePitchCount()
        {
            // add to totals of inning and then reset
            mBallsInInning += mBalls;
            mStrikesInInning += mStrikes;
            mFoulsInInning += mFouls;

            mBalls = 0;
            mStrikes = 0;
            mFouls = 0;
            mStrikesForOutCounting = 0;

        }
        public async void FinishCurrentInning()
        {
            // add totals balls/strikes/fouls for pitcher
            BBPlayer currentpitcher = getCurrentPitcher();
            mScoreManager.registerScore("BaseOnBalls", currentpitcher, mBaseOnBallsInInning);
            mScoreManager.registerScore("Balls", currentpitcher, mBallsInInning);
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
            if (mOuts >= 3)
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
                addMessage($"{mRunnerOn3rdBase.GetShortDisplayString()}\nscores a run.");

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
            actionList.Add(new GameAction_Hit_InFieldPlay(getCurrentBatter(), FieldPositions.batterbox, "Bunt"));
            actionList.Add(new GameAction_Hit_InFieldPlay(getCurrentBatter(), FieldPositions.firstbase, "Single"));
            actionList.Add(new GameAction_Hit_InFieldPlay(getCurrentBatter(), FieldPositions.secondbase, "Double"));
            actionList.Add(new GameAction_Hit_InFieldPlay(getCurrentBatter(), FieldPositions.thirdbase, "Triple"));
            actionList.Add(new GameAction_Hit_InFieldPlay(getCurrentBatter(), FieldPositions.homeplate, "Home Run!"));
            if (runnersOnBase())
            {
                actionList.Add(new GameAction_Hit_OutMultiPlay(getCurrentBatter(), "DoublePlay", "Out, Double Play"));
                if(getNumRunnersOnBase() >= 2)
                    actionList.Add(new GameAction_Hit_OutMultiPlay(getCurrentBatter(), "TriplePlay", "Out, Triple Play"));
                actionList.Add(new GameAction_SacrificeFly(getCurrentBatter()));
                actionList.Add(new GameAction_SacrificeBunt(getCurrentBatter()));
            }
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
        public void addActionsForCatcherPosition(List<IGameAction> actionList)
        {
            BBPlayer catcher = getDefendingTeam().mTeam.getPlayerTypeFromLineUp("C");
            if (mCurrentScoringMode == gameScoringMode.Pitching)
            {   //which actions are possible for the catcher in pitching mode
                actionList.Add(new GameAction_PassedBall(catcher, "PassedBall"));                
            }
            if (mCurrentScoringMode == gameScoringMode.InFieldPlay)
            {   //which actions are possible for the catcher in pitching mode
                actionList.Add(new GameAction_Error(catcher, FieldPositions.catcher ,"Error"));
            }
        }
        public void addActionsForPitcherPosition(List<IGameAction> actionList)
        {
            BBPlayer pitcher = getDefendingTeam().mCurrentlyPitching;
            if (mCurrentScoringMode == gameScoringMode.Pitching)
            {   //which actions are possible for the catcher in pitching mode
                actionList.Add(new GameAction_Balk(pitcher, "Balk"));
                actionList.Add(new GameAction_IntentionalWalk(pitcher, "Intentional Walk"));
            }
            if (mCurrentScoringMode == gameScoringMode.InFieldPlay)
            {   //which actions are possible for the catcher in pitching mode
                actionList.Add(new GameAction_Error(pitcher, FieldPositions.pitcher, "Error"));
            }
        }
        public void addActionsForDefensePosition(List<IGameAction> actionList, FieldPositions position)
        {
            if (mCurrentScoringMode == gameScoringMode.InFieldPlay)
            {   //which actions are possible for the runner on 3rd base in pitching mode
                string poscode = translateFieldPositionToPositionName(position);
                BBPlayer playerInAction = null;
                if (position != FieldPositions.pitcher)
                    playerInAction = getDefendingTeam().mTeam.getPlayerTypeFromLineUp(poscode);
                else
                {
                    playerInAction = getDefendingTeam().mCurrentlyPitching;
                }
                actionList.Add(new GameAction_Error(playerInAction, position, "Error"));
            }
        }
        
        public List<IGameAction> getPossibleGameActions(FieldPositions forPosition)
        {
            List<IGameAction> actionList = new List<IGameAction>();
            if (forPosition == FieldPositions.batterbox)
            {
                addActionsForPlayerBattingPosition(actionList);
            }
            else if (forPosition == FieldPositions.runnerOn1st)
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
            else if (forPosition == FieldPositions.catcher)
            {
                addActionsForCatcherPosition(actionList);
            }
            else if (forPosition == FieldPositions.pitcher)
            {
                addActionsForPitcherPosition(actionList);
            }
            else if (forPosition == FieldPositions.firstbase || forPosition == FieldPositions.secondbase
                || forPosition == FieldPositions.thirdbase || forPosition == FieldPositions.shortstop || forPosition == FieldPositions.leftfield
                || forPosition == FieldPositions.centerfield || forPosition == FieldPositions.rightfield)
                addActionsForDefensePosition(actionList, forPosition);
            return actionList;
        }
        public string translateFieldPositionToPositionName(FieldPositions position)
        {
            string positioncode = "";
            if (position == FieldPositions.catcher) positioncode = "C";
            else if (position == FieldPositions.pitcher) positioncode = "P";
            else if (position == FieldPositions.firstbase) positioncode = "1B";
            else if (position == FieldPositions.secondbase) positioncode = "2B";
            else if (position == FieldPositions.thirdbase) positioncode = "3B";
            else if (position == FieldPositions.shortstop) positioncode = "SS";
            else if (position == FieldPositions.leftfield) positioncode = "LF";
            else if (position == FieldPositions.centerfield) positioncode = "CF";
            else if (position == FieldPositions.rightfield) positioncode = "RF";
            return positioncode;
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
        public bool runnersOnBase()
        {
            return (mRunnerOn1thBase != null || mRunnerOn2ndBase != null || mRunnerOn3rdBase!= null);
        }
        public int getNumRunnersOnBase()
        {
            int onbase = 0;
            if (mRunnerOn1thBase != null)
                onbase++;
            if (mRunnerOn2ndBase != null)
                onbase++;
            if (mRunnerOn3rdBase!= null)
                onbase++;
            return onbase;
        }
        public void EndInfieldPlay()
        {
            //if a defensive play was selected, add it to the scores attached to the currentpitch count
            mScoreManager.updateScoreAtPitchCountForDefensivePlay(mTotalPitchCount,mLastKnownDefensivePlay);

            //back to pitching mode
            mCurrentScoringMode = gameScoringMode.Pitching;
            mRunnerOnIntermediateWaitPosition = null;
            mInfieldPlayAction = null;
        }
        public void tryToMoveIntermediateRunnerToDestination()
        {
            //game action contains his last infield action
            if(mCurrentScoringMode == gameScoringMode.InFieldPlay && mRunnerOnIntermediateWaitPosition != null)
            {
                if(mInfieldPlayAction.mHitAndRunToBase == FieldPositions.firstbase &&
                    mRunnerOn1thBase == null)
                {
                    //single and can move
                    mRunnerOn1thBase = mRunnerOnIntermediateWaitPosition;
                    mRunnerOnIntermediateWaitPosition = null;
                }
                else if (mInfieldPlayAction.mHitAndRunToBase == FieldPositions.secondbase &&
                    mRunnerOn1thBase == null && mRunnerOn2ndBase == null)
                {
                    //double and can move
                    mRunnerOn2ndBase = mRunnerOnIntermediateWaitPosition;
                    mRunnerOnIntermediateWaitPosition = null;
                }
                else if (mInfieldPlayAction.mHitAndRunToBase == FieldPositions.thirdbase&&
                    runnersOnBase() == false)
                {
                    //double and can move
                    mRunnerOn3rdBase= mRunnerOnIntermediateWaitPosition;
                    mRunnerOnIntermediateWaitPosition = null;
                }
            }
        }

    }
}
