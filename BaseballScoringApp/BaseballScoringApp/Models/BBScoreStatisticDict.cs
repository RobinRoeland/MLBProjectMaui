using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class BBScoreStatisticDictEntry : List<BBScoreStatistic>
    {
        BBPlayer mParent;
        public BBScoreStatisticDictEntry(BBPlayer p) {
            mParent = p;
        }
        
        public void registerScore(BBScoreStatistic newScore, ScoreManager.scoretype sctype)
        {
            // als cummulatief, zoek een reeds bestaande score en verhoog het aantal
            // als single, gewoon toevoegen
            if (sctype == ScoreManager.scoretype.cummulative)
            {
                BBScoreStatistic existingScore = getScoreFromList(newScore.ScoreName);
                if (existingScore != null)
                {
                    existingScore.ScoreValue += newScore.ScoreValue;
                }
                else
                {
                    newScore.ScoreIdInInning = Count; // the count of the list is the id
                    Add(newScore);
                }
            }
            else
            {
                newScore.ScoreIdInInning = Count; // the count of the list is the id
                Add(newScore);
            }
        }
        private BBScoreStatistic? getScoreFromList(string scoreName)
        {
            foreach(BBScoreStatistic bScoreStatistic in this)
            {
                if (bScoreStatistic.ScoreName == scoreName)
                    return bScoreStatistic;
            }
            return null;
        }
    }
}
