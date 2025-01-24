using BaseballModelsLib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class BBPlayer : Player
    {

        [JsonIgnore]
        public ImageSource? mPlayerImage; //cached at initial load from URL in player
        public BBPlayer()
        {
            mPlayerImage = null;
        }

        public bool isPitcher()
        {
            /// moet checken op position
            if(Position=="P")
                return true;
            return false;
        }

        public override string ToString()
        {
            return $"{Id}, {Name}({Position}), Team ID: {(TeamId.HasValue ? TeamId.Value.ToString() : "No Team")}";
        }

        public string GetShortDisplayString()
        {
            return $"{Name} ({Rugnummer})";
        }
        public string GetDebugDisplayString()
        {
            return $"{Name} ({Rugnummer}) -> {MLBPersonId}";
        }
        public int getMLBPersonId()
        {
            // MLBPersonId is nullable, de ?? geeft een default value indien MLBPersonId null is
            return MLBPersonId ?? 0;
        }
        public string getImageUrl()
        {
            return $"https://img.mlbstatic.com/mlb-photos/image/upload/d_people:generic:headshot:silo:current.png/r_max/w_180,q_auto:best/v1/people/{MLBPersonId}/headshot/silo/current";
        }
    }
}
