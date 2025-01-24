using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    internal class GameAction_Hit_OutMultiPlay : GameActionBase
    {
        public string scoretobook;
        public GameAction_Hit_OutMultiPlay(BBPlayer forPlayer, string scorename, string TypeOfPlay)
        {
            // TypeOfPlay = double of triple play
            ActionName = "OutMultiPlay";
            scoretobook = scorename;
            ActionDisplayName = TypeOfPlay;
            playerInvolved = forPlayer;
        }

        public override void DoAction(BBGame forGame)
        {
            BBGameProgress gpr = forGame.mGameProgress;
            gpr.mOuts++;

            SoundManager sm = SoundManager.getInstance();
            sm.PlaySound("mp3/heisout.mp3");


            gpr.mCurrentScoringMode = gameScoringMode.InFieldPlay;
            gpr.mInfieldPlayAction = null;

            gpr.addMessage($"{playerInvolved.GetShortDisplayString()}\nHits into {scoretobook} and is Out!");

            // player is the batter who hit in the double or triple play
            gpr.mScoreManager.registerScore(scoretobook, playerInvolved, 1);
            gpr.mScoreManager.registerScore("Out", playerInvolved, 1);

            //move on
            forGame.IncreasePitchCount();
            gpr.restartThePitchCount();
            //next batter from the lineup
            BBTeamGameStatus offeniveteam = gpr.getOffensiveTeam();
            //jur out offseniveteam.MoveToNextBatterInLineUp();
            offeniveteam.mCurrentBatter = null;
        }
    };
}
