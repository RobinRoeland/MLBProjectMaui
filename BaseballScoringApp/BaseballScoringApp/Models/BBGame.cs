using BaseballModelsLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace BaseballScoringApp.Models
{
    public enum FieldPositions { batterbox, runnerOn1st, runnerOn2nd, runnerOn3rd, firstbase, secondbase, thirdbase, pitcher, catcher, shortstop, leftfield, centerfield, rightfield, homeplate };
    public enum InningStatus { InProgress, EndOfInning, EndOfBallGame };
    public class BBGame : Game
    {
        [JsonIgnore] 
        public BBGameProgress mGameProgress;              

        public BBGame()
        {
            mGameProgress = new BBGameProgress(this);
        }

        public void StartGame(int NumInningToPlay)
        {
            BBDataRepository repo = BBDataRepository.getInstance();
            //make object instance links based on id
            BBTeam mHomeTeam = repo.getTeamByID(HomeTeamId);
            BBTeam mAwayTeam = repo.getTeamByID(AwayTeamId);

            mGameProgress.InitialiseNewGame(mHomeTeam, mAwayTeam, NumInningToPlay);

            //set starting pitchers in progress/game
            BBPlayer homepitcher = mHomeTeam.getPlayer(HomeStartingPitcherId);
            BBPlayer awaypitcher = mAwayTeam.getPlayer(AwayStartingPitcherId);

            mHomeTeam.mTeamColor = Colors.LemonChiffon;
            mAwayTeam.mTeamColor = Color.FromArgb("#74cff7");

            mGameProgress.mHomeTeam.mCurrentlyPitching = homepitcher;
            mGameProgress.mAwayTeam.mCurrentlyPitching = awaypitcher;

            Finished = false;

            SoundManager snd = SoundManager.getInstance();
            snd.PlaySound("mp3/playball.mp3");

            mHomeTeam.debugDumpLineUp();
            mAwayTeam.debugDumpLineUp();
        }
        public void DoAction(IGameAction game_action)
        {
            game_action.DoAction(this);
        }
        public void IncreasePitchCount()
        {
            // top of the inning is altijd het home team
            mGameProgress.mTotalPitchCount++;
            if (mGameProgress.mCurrentSideInning == InningSide.Top)
                mGameProgress.mHomeTeam.TotalPitches++;
            else 
                mGameProgress.mAwayTeam.TotalPitches++;
        }
        public void addMessage(string msgToShow)
        {
            mGameProgress.addMessage(msgToShow);
        }
        public bool hasMessagesAvailable()
        {
            return mGameProgress.hasMessagesAvailable();
        }
        public string popMessage()
        {
            return mGameProgress.popMessage();
        }
        public async Task BallGame(BBDataRepository repo)
        {
            Finished = true;
            mGameProgress.BallGame();

            RunsHomeTeam = mGameProgress.mHomeTeam.Runs;
            RunsAwayTeam = mGameProgress.mAwayTeam.Runs;
            HitsHomeTeam = mGameProgress.mHomeTeam.Hits;
            HitsAwayTeam = mGameProgress.mAwayTeam.Hits;
            ErrorsHomeTeam = mGameProgress.mHomeTeam.Errors;
            ErrorsAwayTeam = mGameProgress.mAwayTeam.Errors;

            //update end of game in db
            await repo.sendGameUpdate();
        }

    }
}
