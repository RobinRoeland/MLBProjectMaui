using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class GameAction_Strike : GameActionBase
    {
        public GameAction_Strike(BBPlayer forPlayer)
        {
            ActionName = "Strike";
            ActionDisplayName = "Strike";
            playerInvolved = forPlayer;

        }

        public override void DoAction(BBGame forGame)
        {
            forGame.IncreasePitchCount();
            forGame.mGameProgress.mStrikes++;

            SoundManager sm = SoundManager.getInstance();
            switch (forGame.mGameProgress.mStrikes)
            {
                case 1:
                    {
                        sm.PlaySound("mp3/strikeone.mp3"); break;
                    }
                case 2:
                    {
                        sm.PlaySound("mp3/striketwo.mp3"); break;
                    }
                case 3:
                    {
                        sm.PlaySound("mp3/strike3out.mp3"); break;
                    }
            }
            //add to scoring
            if (forGame.mGameProgress.mStrikes == 3)
            {
                // increase the out and take next batter
                forGame.mGameProgress.Handle_3Strikes_BatterOut();

                forGame.mGameProgress.mScoreManager.registerScore("PStrikeOut", forGame.mGameProgress.getCurrentPitcher(), 1);
                forGame.mGameProgress.mScoreManager.registerScore("HStrikeOut", playerInvolved, 1);

            }
        }
    }
}
