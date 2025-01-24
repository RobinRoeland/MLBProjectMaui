using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class ScoreManager
    {
        public enum scoretype { cummulative, single };
        public Dictionary<string, scoretype> m_ScoreTypeDefinitions;

        public Dictionary<BBPlayer, BBScoreStatisticDictEntry> m_ScoreMapPerPlayer; // key is MLBPersonId

        public BBGame mGameContext;
        public ScoreManager()
        {
            m_ScoreTypeDefinitions = new Dictionary<string, scoretype>();
            m_ScoreMapPerPlayer = new Dictionary<BBPlayer, BBScoreStatisticDictEntry>();
            mGameContext = null;
            initializeStandardScoreTypes();
        }
        public void setGameContext(BBGame parent)
        {
            mGameContext = parent;
        }
        public void initializeStandardScoreTypes()
        {
            // definitie van alle mogelijke score types en of ze cummulatief of individueel zijn
            m_ScoreTypeDefinitions["GamesPlayed"] = scoretype.single; 
            m_ScoreTypeDefinitions["BaseOnBalls"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["HitByPitch"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["Strikes"] = scoretype.single;
            m_ScoreTypeDefinitions["Balls"] = scoretype.single;
            m_ScoreTypeDefinitions["Fouls"] = scoretype.single;
            m_ScoreTypeDefinitions["Balk"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["FoulBall"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["PStrikeOut"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["HStrikeOut"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["RBI"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["Error"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["PlateAppearence"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["Runs"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["PickOff"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["CaughtStealing"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["Hit"] = scoretype.cummulative; 
            m_ScoreTypeDefinitions["Single"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["Double"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["Triple"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["StolenBase"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["HomeRun"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["GrandSlam"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["DoublePlay"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["TriplePlay"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["PassedBall"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["Error"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["IntentionalWalk"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["Out"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["SacrificeBunt"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["SacrificeFly"] = scoretype.cummulative;
            m_ScoreTypeDefinitions["Bunt"] = scoretype.cummulative;
        }
        public void clearAllScores()
        {
            // delete all scores
            m_ScoreMapPerPlayer.Clear(); 
        }

        public BBScoreStatistic registerScore(string scoringType, BBPlayer forPlayer, float scoreValue)
        {
            BBScoreStatistic newScore = new BBScoreStatistic(scoringType,forPlayer,scoreValue);
            // for primary key of score in table
            newScore.GameId = mGameContext.Id;
            newScore.Inning = mGameContext.mGameProgress.mCurrentInning;
            if (mGameContext.mGameProgress.mCurrentSideInning == InningSide.Top)
                newScore.InningTop = 0;
            else 
                newScore.InningTop = 1;
            //the score id within person is set when adding the score in registerscore

            //check if cummulative or single scoring
            scoretype ScoreTypeFornewScore = getScoreTypeForScore(newScore);
            
            //get the dictionaryentry with all scores from scoremap for the given player in the score or create a new one
            BBScoreStatisticDictEntry scoreMapForPlayer = null;
            if (m_ScoreMapPerPlayer.Keys.Contains(newScore.getPlayer()))
            {
                scoreMapForPlayer = m_ScoreMapPerPlayer[newScore.getPlayer()];
            }
            else
            {
                //create new scoremap for new player
                scoreMapForPlayer = new BBScoreStatisticDictEntry(newScore.getPlayer());
                m_ScoreMapPerPlayer[newScore.getPlayer()] = scoreMapForPlayer;
            }
            //now we have the scoreMap of the player
            BBScoreStatistic updatedScore = scoreMapForPlayer.registerScore(newScore, ScoreTypeFornewScore);
            // atpitchcount is used at end of play to update all registered score with same pitchcount with defensive play
            updatedScore.AtPitchCount = mGameContext.mGameProgress.mTotalPitchCount;
            return newScore;
        }
        private scoretype getScoreTypeForScore(BBScoreStatistic scoreStatistic)
        {
            if (m_ScoreTypeDefinitions.Keys.Contains(scoreStatistic.ScoreName))
                return m_ScoreTypeDefinitions[scoreStatistic.ScoreName];
            //default single
            return scoretype.single;
        }
        public async Task publishScoreList()
        {
            BBDataRepository repo = BBDataRepository.getInstance();
            //make a flat list of all scores for sending
            List<BBScoreStatistic> scoresList = new List<BBScoreStatistic>();
            foreach (BBPlayer player in m_ScoreMapPerPlayer.Keys)
            {
                BBScoreStatisticDictEntry entriesForPlayer = m_ScoreMapPerPlayer[player];
                foreach(BBScoreStatistic statistic in entriesForPlayer)
                {
                    scoresList.Add(statistic);
                }
            }
            //now send to repo for sending
            bool returnvalue = await repo.sendScoreList(scoresList);
            if (!returnvalue)
                mGameContext.addMessage("Failed to send scores to cloud.");            
        }
        public void updateScoreAtPitchCountForDefensivePlay(int atPitchCount, string defensiveplay)
        {
            //for each player, update all scores at pitchcount with defensive play string
            foreach (BBPlayer player in m_ScoreMapPerPlayer.Keys)
            {
                BBScoreStatisticDictEntry entriesForPlayer = m_ScoreMapPerPlayer[player];
                foreach (BBScoreStatistic statistic in entriesForPlayer)
                {
                    if(statistic.AtPitchCount == atPitchCount)
                        statistic.DefensivePlay = defensiveplay;
                }
            }
        }
    }
}
