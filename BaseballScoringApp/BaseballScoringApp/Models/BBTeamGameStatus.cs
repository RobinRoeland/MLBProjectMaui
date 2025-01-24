using BaseballModelsLib.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        
        // current batter is set based on currentbatter number but will be set to null if the batter move to a base
        // the action then sets this mcurrentbatter to null, in this way, the game knows to ask for a new batter
        // in form (currentbatternumber always stays filled in to remember where we were in the lineup.
        public BBPlayer? mCurrentBatter;

        public BBTeamGameStatus(BBTeam team, BBGameProgress parent) {
            mParent = parent;
            mTeam = team;
            mCurrentlyPitching = null;
            mCurrentBatter = null;
            InningScores = new List<int>();
        }  
        public void startGame()
        {
            Runs = 0;
            Hits = 0;
            Errors = 0;
            TotalPitches = 0;
            mCurrentBatterNumber = 0;
            // jur out MoveToNextBatterInLineUp();// this also makes sure the plate appearance at bat score is added for the first
            mCurrentBatter = null;
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
            if (mCurrentBatterNumber - 1 < 0)
                return null;
            return mTeam.mLineUpList[mCurrentBatterNumber - 1];
        }

        public BBPlayer MoveToNextBatterInLineUp()
        {
            if (mCurrentBatter != null)
                Debug.WriteLine("mCurrentBatter not null and trying to move to the next batter. calling function should set the currentbatter to null first.");
                   
            mCurrentBatterNumber++;
            if (mCurrentBatterNumber > 9)
                mCurrentBatterNumber = 1;

            mParent.mBattersInInning++;

            // add scoring counting for at plate appearance
            BBScoreStatistic st = mParent.mScoreManager.registerScore("PlateAppearence", getCurrentBatter(), 1);
            // only for plateappearance, the next batter is selected before the total pitchcount is increased
            //increase it here
            st.AtPitchCount = mParent.mTotalPitchCount + 1;

            //defines the new batter for the game
            mCurrentBatter = getCurrentBatter();
            return mCurrentBatter;
        }
        public void addRunScoreToInning(int inning, int numRunsScore=1)
        {
            // adds a run to the current inning score in the scoreboard array of the team
            if (inning > 0 && inning <= InningScores.Count)
            {
                InningScores[inning - 1]+=numRunsScore;
                Runs+=numRunsScore;
            }
        }
    }
}
