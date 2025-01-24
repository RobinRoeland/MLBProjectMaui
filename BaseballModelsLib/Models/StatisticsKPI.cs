using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseballModelsLib.Models
{
    public class StatisticsKPI
    {
        public string StatisticsName { get; set; }

        public float StatisticsValue { get; set; }

        public int SpelerId { get; set; }

        public StatisticsKPI(string statisticsName, float statisticsValue, int spelerId)
        {
            StatisticsName = statisticsName;
            StatisticsValue = statisticsValue;
            SpelerId = spelerId;
        }
    }
}
