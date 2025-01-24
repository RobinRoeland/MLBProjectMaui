using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class GameAction_SacrificeBunt : GameActionBase
    {
        public GameAction_SacrificeBunt(BBPlayer forPlayer)
        {
            ActionName = "SacrificeBunt";
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

                forGame.addMessage($"{playerInvolved.Name} ({playerInvolved.Rugnummer.ToString()})\nBunts Out.");

                gpr.mScoreManager.registerScore(ActionName, playerInvolved, 1);
                gpr.mScoreManager.registerScore("Out", playerInvolved, 1);
            }

        }

    }
}