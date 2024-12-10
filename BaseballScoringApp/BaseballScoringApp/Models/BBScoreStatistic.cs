using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseballModelsLib.Models;
using System.Text.Json.Serialization;

namespace BaseballScoringApp.Models
{
    public class BBScoreStatistic : ScoreStatistic
    {
        [JsonIgnore]
        private BBPlayer _mPlayer;
        [JsonIgnore]
        public BBPlayer mPlayer
        {
            get => _mPlayer;
            set
            {
                _mPlayer = value;
                if (_mPlayer != null)
                {
                    PersonMLBId = (int)_mPlayer.MLBPersonId;
                }

            }
        }
        public BBScoreStatistic(string scoreName, BBPlayer forplayer, float StatisticAmount)     
        {
            ScoreValue = StatisticAmount;
            ScoreName = scoreName;
            mPlayer = forplayer;            
        }
        public BBPlayer? getPlayer()
        {
            if(mPlayer is BBPlayer)
                return mPlayer as BBPlayer;
            return null;
        }
    }
}
