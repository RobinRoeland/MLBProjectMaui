using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class GameAction_PickOff : GameActionBase
    {
        FieldPositions mWhichBase;
        public GameAction_PickOff(BBPlayer forPlayer, FieldPositions ErrorOnbase, string display)
        {
            ActionName = "Error";
            mWhichBase = ErrorOnbase;
            ActionDisplayName = display;
            playerInvolved = forPlayer;
        }

        public override void DoAction(BBGame forGame)
        {
            BBGameProgress gpr = forGame.mGameProgress;

            gpr.mOuts++;

            SoundManager sm = SoundManager.getInstance();
            sm.PlaySound("mp3/heisout.mp3");

            switch (mWhichBase)
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

            gpr.mScoreManager.registerScore("PickOff", gpr.getCurrentPitcher(), 1);
            gpr.mScoreManager.registerScore("PickOff", playerInvolved, 1);
            gpr.mScoreManager.registerScore("Out", playerInvolved, 1);

        }
    };
}