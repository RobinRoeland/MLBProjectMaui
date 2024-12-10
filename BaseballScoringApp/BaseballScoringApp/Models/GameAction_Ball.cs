using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class GameAction_Ball : GameActionBase
    {
        public GameAction_Ball(BBPlayer forPlayer)
        {
            ActionName = "Ball";
            ActionDisplayName = "Ball";
            playerInvolved = forPlayer;
        }

        public override void DoAction(BBGame forGame)
        {
            forGame.IncreasePitchCount();
            forGame.mGameProgress.mBalls++;
            SoundManager sm = SoundManager.getInstance();
            switch (forGame.mGameProgress.mBalls)
            {
                case 1:
                    {
                        sm.PlaySound("mp3/BallOne.mp3"); break;
                    }
                case 2:
                    {
                        sm.PlaySound("mp3/BallOne.mp3"); break;
                    }
                case 3:
                    {
                        sm.PlaySound("mp3/BallThree.mp3"); break;
                    }
                case 4:
                    {
                        sm.PlaySound("mp3/BallFourTakeBase.mp3"); break;
                    }
            }
            //scoring of balls only happens end of inning based on totals

            if (forGame.mGameProgress.mBalls > 3)
            {
                GameAction_WalkBatter walkBatterAction = new GameAction_WalkBatter(playerInvolved, "BaseOnBalls");
                walkBatterAction.DoAction(forGame);
            }

        }
    }
}
