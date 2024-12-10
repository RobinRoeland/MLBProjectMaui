using BaseballModelsLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballScoringApp.Models
{
    public class BBPlayer : Player
    {
        public BBPlayer()
        {
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
            return $"{Name}({Rugnummer})";
        }
        public int getMLBPersonId()
        {
            // MLBPersonId is nullable, de ?? geeft een default value indien MLBPersonId null is
            return MLBPersonId ?? 0;
        }
    }
}
