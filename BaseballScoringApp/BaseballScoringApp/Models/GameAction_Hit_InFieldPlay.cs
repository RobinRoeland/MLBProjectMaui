using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class GameAction_Hit_InFieldPlay : GameActionBase
    {
        FieldPositions mHitAndRunToBase;
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

            forGame.IncreasePitchCount();
            
            //bring the scoring form and game in InFieldPlay mode, until end of play clicked
            gpr.mCurrentScoringMode = gameScoringMode.InFieldPlay;

            gpr.mRunnerOnIntermediateWaitPosition = playerInvolved;
            gpr.getOffensiveTeam().MoveToNextBatterInLineUp();

            if (mHitAndRunToBase == FieldPositions.firstbase)
            {
                gpr.mScoreManager.registerScore("Single", playerInvolved, 1);                
            }
            else if (mHitAndRunToBase == FieldPositions.secondbase)
            {
                gpr.mScoreManager.registerScore("Double", playerInvolved, 1);
            }
            else if (mHitAndRunToBase == FieldPositions.thirdbase)
            {
                gpr.mScoreManager.registerScore("Triple", playerInvolved, 1);

            }
            else if (mHitAndRunToBase == FieldPositions.homeplate)
            {
                // to do code homerun
                forGame.mGameProgress.mScoreManager.registerScore("HomeRun", playerInvolved, 1);
            }
            //do what has to be done
            //if(mWhichBase == FieldPositions.runnerOn3rd)
        }
    }
}
