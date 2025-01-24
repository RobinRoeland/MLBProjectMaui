using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    internal class GameAction_Error : GameActionBase
    {
        FieldPositions mWhichBase;
        public GameAction_Error(BBPlayer forPlayer, FieldPositions pickedOfOnbase, string display)
        {
            ActionName = "Pickoff";
            mWhichBase = pickedOfOnbase;
            ActionDisplayName = display;
            playerInvolved = forPlayer;
        }

        public override void DoAction(BBGame forGame)
        {
            BBGameProgress gpr = forGame.mGameProgress;

            gpr.getDefendingTeam().Errors++;

            //SoundManager sm = SoundManager.getInstance();
            //sm.PlaySound("mp3/heisout.mp3");

            gpr.mScoreManager.registerScore("Error", playerInvolved, 1);
        }
    };
}