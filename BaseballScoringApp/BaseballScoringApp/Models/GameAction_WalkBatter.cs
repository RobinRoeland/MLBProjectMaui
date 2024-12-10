using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class GameAction_WalkBatter : GameActionBase
    {
        public GameAction_WalkBatter(BBPlayer forPlayer, string scorename)
        {
            ActionName = "Walk";
            ActionDisplayName = scorename;
            playerInvolved = forPlayer;
        }
        public override void DoAction(BBGame forGame)
        {
            // a walk moves the batter and resets the counts
            BBGameProgress gpr = forGame.mGameProgress;
            gpr.restartThePitchCount();

            BBTeamGameStatus offensiveTeam = gpr.getOffensiveTeam();

            if (gpr.mRunnerOn1thBase != null)
            {
                //what if first is occupied
                gpr.Handle_AdvanceRunnerOn1st();
            }
            // now move the batter to 1st, take next batter !
            gpr.mRunnerOn1thBase = offensiveTeam.getCurrentBatter();
            offensiveTeam.MoveToNextBatterInLineUp();
            
            if(ActionDisplayName== "BaseOnBalls")
                forGame.addMessage("Ball 4\nBatter takes a base");
            else // HitByPitch
                forGame.addMessage($"{playerInvolved.Name} ({playerInvolved.Rugnummer.ToString()})\nHit By Pitch, move to 1st base.");
            
            // "BaseOnBalls" or "HitByPitch"
            gpr.mScoreManager.registerScore(ActionDisplayName, playerInvolved, 1);
        }
    }
}
