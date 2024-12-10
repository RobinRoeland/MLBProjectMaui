using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class GameAction_StealBase : GameActionBase
    {
        FieldPositions mWhichBase;
        public GameAction_StealBase(BBPlayer forPlayer, FieldPositions stealbase, string display)
        {
            ActionName = "StealBase";
            playerInvolved = forPlayer;
            mWhichBase = stealbase;
            ActionDisplayName = display;
        }

        public override void DoAction(BBGame forGame)
        {
            BBGameProgress gpr = forGame.mGameProgress;
            //remove from current base
            if (gpr.mRunnerOn1thBase == playerInvolved)
                gpr.mRunnerOn1thBase = null;
            else if (gpr.mRunnerOn2ndBase== playerInvolved)
                gpr.mRunnerOn2ndBase= null;
            if (gpr.mRunnerOn3rdBase== playerInvolved)
                gpr.mRunnerOn3rdBase = null;
            // move to new base
            if (mWhichBase == FieldPositions.secondbase)
            {
                gpr.mRunnerOn2ndBase = playerInvolved;
            }
            else if (mWhichBase == FieldPositions.thirdbase)
            {
                gpr.mRunnerOn3rdBase = playerInvolved;
            }
            else if (mWhichBase == FieldPositions.homeplate)
            {
                // a run scores
                BBTeamGameStatus offsensiveteam = gpr.getOffensiveTeam();
                offsensiveteam.addRunScoreToInning(gpr.mCurrentInning);
                gpr.addMessage($"{playerInvolved.Name} ({playerInvolved.Rugnummer.ToString()})\nscores a run.");
                //Add scoring for batter
                gpr.mScoreManager.registerScore("Runs", playerInvolved, 1);
            }
            gpr.mScoreManager.registerScore("StolenBase", playerInvolved, 1);
        }
    }
}
