using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class GameAction_PassedBall : GameActionBase
    {
        FieldPositions mWhichBase;
        public GameAction_PassedBall(BBPlayer forPlayer, string display)
        {
            ActionName = "PassedBall";
            ActionDisplayName = display;
            playerInvolved = forPlayer;
        }

        public override void DoAction(BBGame forGame)
        {
            //only leads to a score note for the catcher, no increase
            
            // there is no increase of the pitch count !
            // the ball can be passed and be a strike or ball. 
            // passed ball only makes the score note, the score of ball of strike still needs to be done by user
            
            BBGameProgress gpr = forGame.mGameProgress;
            gpr.mScoreManager.registerScore("PassedBall", playerInvolved, 1);
        }
    };
}
