using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class GameAction_CaughtOutInPlay : GameActionBase
    {
        public GameAction_CaughtOutInPlay(BBPlayer forPlayer, string display)
        {
            ActionName = "CaughtOutOnBase";
            ActionDisplayName = display;
            playerInvolved = forPlayer;
        }

        public override void DoAction(BBGame forGame)
        {
            BBGameProgress gpr = forGame.mGameProgress;
            if (gpr != null)
            {
                if (gpr.mRunnerOn1thBase == playerInvolved)
                {
                    gpr.mOuts++;
                    gpr.mRunnerOn1thBase = null;
                    gpr.addMessage($"{playerInvolved.Name} ({playerInvolved.Rugnummer.ToString()})\nOut on 1st base.");
                    gpr.mScoreManager.registerScore("Out", playerInvolved, 1);
                }
                else if (gpr.mRunnerOn2ndBase== playerInvolved)
                {
                    gpr.mOuts++;
                    gpr.mRunnerOn2ndBase= null;
                    gpr.addMessage($"{playerInvolved.Name} ({playerInvolved.Rugnummer.ToString()})\nOut on 2nd base.");
                    gpr.mScoreManager.registerScore("Out", playerInvolved, 1);
                }
                else if (gpr.mRunnerOn3rdBase== playerInvolved)
                {
                    gpr.mOuts++;
                    gpr.mRunnerOn3rdBase= null;
                    gpr.addMessage($"{playerInvolved.Name} ({playerInvolved.Rugnummer.ToString()})\nOut on 3rd base.");
                    gpr.mScoreManager.registerScore("Out", playerInvolved, 1);
                }
                //todo add scoring if needed
            }
        }
    };
}
