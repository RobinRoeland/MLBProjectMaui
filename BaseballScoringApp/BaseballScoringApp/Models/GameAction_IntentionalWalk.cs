using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class GameAction_IntentionalWalk : GameActionBase
    {
        public GameAction_IntentionalWalk(BBPlayer forPlayer, string display)
        {
            ActionName = "Pickoff";
            ActionDisplayName = display;
            playerInvolved = forPlayer;
        }

        public override void DoAction(BBGame forGame)
        {
            BBGameProgress gpr = forGame.mGameProgress;

            BBTeamGameStatus offensiveTeam = gpr.getOffensiveTeam();

            if (gpr.mRunnerOn1thBase != null)
            {
                //what if first is occupied
                gpr.Handle_AdvanceRunnerOn1st();
            }
            // now move the batter to 1st, take next batter !
            BBPlayer currentBatter = offensiveTeam.getCurrentBatter();
            gpr.mRunnerOn1thBase = currentBatter;
            //jur out offensiveTeam.MoveToNextBatterInLineUp();
            offensiveTeam.mCurrentBatter = null;

            forGame.addMessage("Pitcher walks the batter intentionally");
            gpr.mBaseOnBallsInInning++;

            gpr.restartThePitchCount();

            gpr.mScoreManager.registerScore("IntentionalWalk", playerInvolved, 1); //pitcher
            gpr.mScoreManager.registerScore("IntentionalWalk", currentBatter, 1);


        }
    };
}