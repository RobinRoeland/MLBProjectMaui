using BaseballModelsLib.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class BBTeamGameStatus
    {
        public BBGameProgress mParent;

        public BBTeam mTeam;

        public int Runs; //R
        public int Hits; //H
        public int Errors; //E

        public int TotalPitches;
        public List<int> InningScores;

        public BBPlayer? mCurrentlyPitching;
        public int mCurrentBatterNumber; // 1 to 9

        public BBTeamGameStatus(BBTeam team, BBGameProgress parent) {
            mParent = parent;
            mTeam = team;
            mCurrentlyPitching = null;
            InningScores = new List<int>();
        }  
        public void startGame()
        {
            Runs = 0;
            Hits = 0;
            Errors = 0;
            TotalPitches = 0;
            mCurrentBatterNumber = 0;
            MoveToNextBatterInLineUp();// this also makes sure the plate appearance at bat score is added for the first
            InningScores.Clear();
            for (int i = 0; i < 9; i++)
            {
                InningScores.Add(0);
            }
        }
        public int getInningScore(int inning)
        {
            if (inning >= InningScores.Count)
            {
                return 0;
            }
            return InningScores[inning - 1];
        }
        public BBPlayer getCurrentBatter()
        {
            return mTeam.mLineUpList[mCurrentBatterNumber - 1];
        }

        public BBPlayer MoveToNextBatterInLineUp()
        {
            mCurrentBatterNumber++;
            if (mCurrentBatterNumber > 9)
                mCurrentBatterNumber = 1;

            mParent.mBattersInInning++;

            // add scoring counting for at plate appearance
            mParent.mScoreManager.registerScore("PlateAppearence", getCurrentBatter(), 1);

            return getCurrentBatter();
        }
        public void addRunScoreToInning(int inning)
        {
            // adds a run to the current inning score in the scoreboard array of the team
            if (inning > 0 && inning <= InningScores.Count)
            {
                InningScores[inning - 1]++;
                Runs++;
            }
        }
    }
}
