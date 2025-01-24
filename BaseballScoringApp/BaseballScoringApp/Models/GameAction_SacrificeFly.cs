using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class GameAction_SacrificeFly : GameActionBase
    {
        public GameAction_SacrificeFly(BBPlayer forPlayer)
        {
            ActionName = "SacrificeFly";
            ActionDisplayName = ActionName;
            playerInvolved = forPlayer;
        }

        public override void DoAction(BBGame forGame)
        {
            BBGameProgress gpr = forGame.mGameProgress;
            if (gpr != null)
            {
                gpr.mOuts++;
                gpr.restartThePitchCount();

                gpr.mCurrentScoringMode = gameScoringMode.InFieldPlay;
                gpr.mInfieldPlayAction = null;

                //moving to next batter happens at end of play in infield mode
                //currentbatter remains the one that started the infieldplay

                forGame.addMessage($"{playerInvolved.Name} ({playerInvolved.Rugnummer.ToString()})\nFlies Out.");

                gpr.mScoreManager.registerScore(ActionName, playerInvolved, 1);
                gpr.mScoreManager.registerScore("Out", playerInvolved, 1);
            }

        }

    }
}
