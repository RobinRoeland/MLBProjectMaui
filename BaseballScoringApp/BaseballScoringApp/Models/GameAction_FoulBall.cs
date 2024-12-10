using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class GameAction_FoulBall : GameActionBase
    {
        public GameAction_FoulBall(BBPlayer forPlayer)
        {
            ActionName = "FoulBall";
            ActionDisplayName = "Foul Ball";
            playerInvolved = forPlayer;
        }

        public override void DoAction(BBGame forGame)
        {
            forGame.IncreasePitchCount();
            if (forGame.mGameProgress.mStrikes < 2)
                forGame.mGameProgress.mStrikes++;
            forGame.mGameProgress.mFouls++;
            forGame.mGameProgress.mScoreManager.registerScore("FoulBall", playerInvolved, 1);
        }
    }
}
