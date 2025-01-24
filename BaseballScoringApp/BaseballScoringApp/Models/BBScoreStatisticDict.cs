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
        
        public BBScoreStatistic? registerScore(BBScoreStatistic newScore, ScoreManager.scoretype sctype)
        {
            // als cummulatief, zoek een reeds bestaande score en verhoog het aantal
            // als single, gewoon toevoegen
            // returns the score that was updated (cummulatieve/single)
            BBScoreStatistic? scoreReturn = null;
            if (sctype == ScoreManager.scoretype.cummulative)
            {
                BBScoreStatistic existingScore = getScoreFromList(newScore.ScoreName);
                if (existingScore != null)
                {
                    existingScore.ScoreValue += newScore.ScoreValue;
                    scoreReturn = existingScore;
                }
                else
                {
                    newScore.ScoreIdInInning = Count; // the count of the list is the id
                    Add(newScore);
                    scoreReturn = newScore;
                }
            }
            else
            {
                newScore.ScoreIdInInning = Count; // the count of the list is the id
                Add(newScore);
                scoreReturn = newScore;
            }
            return scoreReturn;
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
