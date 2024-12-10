using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class GameAction_MoveToBase : GameActionBase
    {
        FieldPositions mToBase;
        FieldPositions mFromBase;
        public GameAction_MoveToBase(BBPlayer forPlayer, FieldPositions currentbase, FieldPositions destinationbase, string display)
        {
            ActionName = "MoveToBase";
            playerInvolved = forPlayer;
            mFromBase = currentbase;
            mToBase = destinationbase;
            ActionDisplayName = display;
        }

        public override void DoAction(BBGame forGame)
        {
            // action used to move a player from his current base to another given base or home.
            // bases are available and path is free when we come a this point
            // this is just moving the runner, or scores a run if runs to homeplate
            BBGameProgress gpr = forGame.mGameProgress;
            if (mToBase != FieldPositions.homeplate)
            {
                //move to new pos on field
                if (mToBase == FieldPositions.secondbase)
                    gpr.mRunnerOn2ndBase = playerInvolved;
                else if (mToBase == FieldPositions.thirdbase)
                    gpr.mRunnerOn3rdBase = playerInvolved;
            }
            else
            {
                //run scores
            }
            //remove form old pos on field
            if (mFromBase == FieldPositions.firstbase)
                gpr.mRunnerOn1thBase = null;
            else if (mFromBase == FieldPositions.secondbase)
                gpr.mRunnerOn2ndBase = null;
            else if (mFromBase == FieldPositions.thirdbase)
                gpr.mRunnerOn3rdBase = null;

        }
    };
}