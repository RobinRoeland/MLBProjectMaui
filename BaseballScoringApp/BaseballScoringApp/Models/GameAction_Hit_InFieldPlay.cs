using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class GameAction_Hit_InFieldPlay : GameActionBase
    {
        public FieldPositions mHitAndRunToBase;
        public GameAction_Hit_InFieldPlay(BBPlayer forPlayer, FieldPositions runtobase, string display)
        {
            ActionName = "InFieldHit";
            playerInvolved = forPlayer;
            mHitAndRunToBase = runtobase;
            ActionDisplayName = display;
        }

        public override void DoAction(BBGame forGame)
        {
            BBGameProgress gpr = forGame.mGameProgress;
            SoundManager sm = SoundManager.getInstance();

            forGame.IncreasePitchCount();
            
            //bring the scoring form and game in InFieldPlay mode, until end of play clicked
            gpr.mCurrentScoringMode = gameScoringMode.InFieldPlay;
            gpr.mInfieldPlayAction = this;

            BBTeamGameStatus offensiveteam = gpr.getOffensiveTeam();
            // reset pitch count a new batter is up
            gpr.restartThePitchCount();
            // currentbatter is set to null at end of play button to force picking next batter

            //moving to next batter happens at end of play in infield mode
            //currentbatter remains the one that started the infieldplay            

            if (mHitAndRunToBase == FieldPositions.firstbase)
            {
                if (gpr.mRunnerOn1thBase != null)
                {
                    gpr.mRunnerOnIntermediateWaitPosition = playerInvolved;
                }
                else
                {
                    gpr.mRunnerOn1thBase = playerInvolved;
                }
                sm.PlaySound("mp3/hitball.mp3");
                gpr.mScoreManager.registerScore("Hit", playerInvolved, 1); 
                gpr.mScoreManager.registerScore("Single", playerInvolved, 1);                
            }
            else if (mHitAndRunToBase == FieldPositions.secondbase)
            {
                if (gpr.mRunnerOn1thBase != null || gpr.mRunnerOn2ndBase != null)
                {
                    gpr.mRunnerOnIntermediateWaitPosition = playerInvolved;
                }
                else
                {
                    gpr.mRunnerOn2ndBase = playerInvolved;
                }
                sm.PlaySound("mp3/hitball.mp3");
                gpr.mScoreManager.registerScore("Hit", playerInvolved, 1);
                gpr.mScoreManager.registerScore("Double", playerInvolved, 1);
            }
            else if (mHitAndRunToBase == FieldPositions.thirdbase)
            {
                if (gpr.runnersOnBase())
                {
                    gpr.mRunnerOnIntermediateWaitPosition = playerInvolved;
                }
                else
                {
                    gpr.mRunnerOn3rdBase= playerInvolved;
                }

                sm.PlaySound("mp3/hitball.mp3");
                gpr.mScoreManager.registerScore("Hit", playerInvolved, 1);
                gpr.mScoreManager.registerScore("Triple", playerInvolved, 1);
            }
            else if (mHitAndRunToBase == FieldPositions.batterbox)
            {
                //bunt scenario
                if (gpr.mRunnerOn1thBase != null)
                {
                    gpr.mRunnerOnIntermediateWaitPosition = playerInvolved;
                }
                else
                {
                    gpr.mRunnerOn1thBase= playerInvolved;
                }

                sm.PlaySound("mp3/bunt.mp3");
                gpr.mScoreManager.registerScore("Bunt", playerInvolved, 1);
            }
            else if (mHitAndRunToBase == FieldPositions.homeplate)
            {
                sm.PlaySound("mp3/homerun.mp3");

                // with home run, there is no infield play mode, move directly to new batter
                gpr.mCurrentScoringMode = gameScoringMode.Pitching;
                gpr.mInfieldPlayAction = null;
                // code for homerun handling of RBI and grandslam
                int RBIRunsScore = 0;
                if (gpr.mRunnerOn1thBase != null)
                {
                    RBIRunsScore++;
                    gpr.mScoreManager.registerScore("Runs", gpr.mRunnerOn1thBase, 1);
                    gpr.addMessage($"{gpr.mRunnerOn1thBase.GetShortDisplayString()}\nScores from 1st base.");
                    gpr.mRunnerOn1thBase= null;
                }
                if (gpr.mRunnerOn2ndBase != null)
                {
                    RBIRunsScore++;
                    gpr.mScoreManager.registerScore("Runs", gpr.mRunnerOn2ndBase, 1);
                    gpr.addMessage($"{gpr.mRunnerOn2ndBase.GetShortDisplayString()}\nScores from 2nd base.");
                    gpr.mRunnerOn2ndBase= null;
                }
                if (gpr.mRunnerOn3rdBase != null)
                {
                    RBIRunsScore++;
                    gpr.mScoreManager.registerScore("Runs", gpr.mRunnerOn3rdBase, 1);
                    gpr.addMessage($"{gpr.mRunnerOn3rdBase.GetShortDisplayString()}\nScores from 3rd base.");
                    gpr.mRunnerOn3rdBase = null;
                }
                if(RBIRunsScore > 0)
                    forGame.mGameProgress.mScoreManager.registerScore("RBI", playerInvolved, RBIRunsScore);
                if (RBIRunsScore == 3)
                {
                    gpr.addMessage($"{playerInvolved.GetShortDisplayString()}\nGrand Slam Home Run !");
                    forGame.mGameProgress.mScoreManager.registerScore("GrandSlam", playerInvolved, 1);
                }
                else 
                    gpr.addMessage($"{playerInvolved.GetShortDisplayString()}\nHome Run !");

                //add the runs
                offensiveteam.addRunScoreToInning(gpr.mCurrentInning, RBIRunsScore + 1);
                
                forGame.mGameProgress.mScoreManager.registerScore("Runs", playerInvolved, 1);// a run for the batter
                forGame.mGameProgress.mScoreManager.registerScore("HomeRun", playerInvolved, 1); // a homerun for the batter
                forGame.mGameProgress.mScoreManager.registerScore("Hit", playerInvolved, 1);
                
                //move to next batter
                offensiveteam.mCurrentBatter = null;
            }
        }
    }
}
