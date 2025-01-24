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
            SoundManager sm = SoundManager.getInstance();
            sm.PlaySound("mp3/FoulBall.mp3");

            forGame.IncreasePitchCount();
            if (forGame.mGameProgress.mStrikesForOutCounting < 2)
                forGame.mGameProgress.mStrikesForOutCounting++;
            forGame.mGameProgress.mFouls++;
            forGame.mGameProgress.mScoreManager.registerScore("FoulBall", playerInvolved, 1);
        }
    }
}
