using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class GameAction_Balk : GameActionBase
    {
        public GameAction_Balk(BBPlayer forPlayer, string display)
        {
            ActionName = "Balk";
            ActionDisplayName = display;
            playerInvolved = forPlayer;
        }

        public override void DoAction(BBGame forGame)
        {
            BBGameProgress gpr = forGame.mGameProgress;
            SoundManager sm = SoundManager.getInstance();
            sm.PlaySound("mp3/Balk.mp3");

            BBTeamGameStatus offsensiveteam = gpr.getOffensiveTeam();
            if (gpr.mRunnerOn3rdBase!= null)
            {
                //what if first is occupied
                offsensiveteam.addRunScoreToInning(gpr.mCurrentInning);
                gpr.addMessage($"{gpr.mRunnerOn3rdBase.GetShortDisplayString()}\nscores a run.");
                gpr.mScoreManager.registerScore("Runs", gpr.mRunnerOn3rdBase, 1);
                gpr.mRunnerOn3rdBase = null;
            }
            if (gpr.mRunnerOn2ndBase!= null)
            {
                gpr.mRunnerOn3rdBase = gpr.mRunnerOn2ndBase;
                gpr.mRunnerOn2ndBase = null;
            }
            if (gpr.mRunnerOn1thBase!= null)
            {
                gpr.mRunnerOn2ndBase= gpr.mRunnerOn1thBase;
                gpr.mRunnerOn1thBase= null;
            }
            
            gpr.mScoreManager.registerScore("Balk", playerInvolved, 1);
        }
    };
}