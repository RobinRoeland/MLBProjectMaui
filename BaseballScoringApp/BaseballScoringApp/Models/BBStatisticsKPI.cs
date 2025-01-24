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
    public class BBStatisticsKPI : StatisticsKPI
    {
        [JsonIgnore]
        public List<BBStatisticsKPI> detailScores { get; set; } = null;

        public BBStatisticsKPI(string statisticsName, float statisticsValue, int spelerId) : base(statisticsName, statisticsValue, spelerId) { }

    }
}
