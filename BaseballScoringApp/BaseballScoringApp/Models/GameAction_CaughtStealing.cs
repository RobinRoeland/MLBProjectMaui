using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    class GameAction_CaughtStealing : GameActionBase
    {
        public FieldPositions mFieldPos;
        public GameAction_CaughtStealing(BBPlayer forPlayer, FieldPositions onPosition, string displayname)
        {
            ActionName = "CaughtStealing";
            ActionDisplayName = displayname;
            playerInvolved = forPlayer;
            mFieldPos = onPosition; 
        }
        public override void DoAction(BBGame forGame)
        {
            // a walk moves the batter and resets the counts
            BBGameProgress gpr = forGame.mGameProgress;
            gpr.mOuts++;
            switch (mFieldPos)
            {
                case FieldPositions.firstbase:
                    gpr.mRunnerOn1thBase = null;
                    break;
                case FieldPositions.secondbase:
                    gpr.mRunnerOn2ndBase = null;
                    break;
                case FieldPositions.thirdbase:
                    gpr.mRunnerOn3rdBase = null;
                    break;
            }
            SoundManager sm = SoundManager.getInstance();
            sm.PlaySound("mp3/heisout.mp3");
            gpr.mScoreManager.registerScore("CaughtStealing", playerInvolved, 1);
        }
    }
}
