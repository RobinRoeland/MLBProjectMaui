using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BaseballModelsLib.Models
{
    public class KPIFactory
    {
        public Dictionary<string, RekenKPI> mKPIs;

        public KPIFactory()
        {
            mKPIs = new Dictionary<string, RekenKPI>()
            {
                { "GamesPlayed", new RekenKPI_Som("GamesPlayed") },
                { "RBI", new RekenKPI_Som("RBI") },
                { "BA", new RekenKPI_BatAvg("BA") },
                { "OBP", new RekenKPI_OBP("OBP") },
                { "SLG", new RekenKPI_SLG("SLG") },
                { "OPS", new RekenKPI_OPS("OPS") }
            };
        }
    }

    public class RekenKPI
    {
        public string KPIName { get; set; } = "";
        public List<string> UsedScores { get; set; } = new List<string>();

        public RekenKPI(string name)
        {
            KPIName = name;
        }

        public virtual StatisticsKPI Calculate(List<ScoreStatistic> algemeneScoreLijstUitTabel) { return null; }

        public List<string> getUsedScoresInCalculation() { 
            return UsedScores;
        }
    }

    public class RekenKPI_Som : RekenKPI
    {
        public RekenKPI_Som(string name) : base(name)
        {
            UsedScores.Add(name);
        }

		public override StatisticsKPI Calculate(List<ScoreStatistic> allScores)
        {
            List<ScoreStatistic> allScoresForKPI = allScores.Where(score => score.ScoreName == KPIName).ToList();   // KPIName uit de basisklasse RekenKPI die gezet is in de constructor  
            float som = allScoresForKPI.Select(score => score.ScoreValue).Sum();

            return new StatisticsKPI(KPIName, som, allScores.First().PersonMLBId);
        }
    }

    public class RekenKPI_BatAvg : RekenKPI
    {
        public RekenKPI_BatAvg(string name) : base(name)
        {
            UsedScores = new List<string>()
            {
                "Hit",
                "PlateAppearence"
            };
        }

		public override StatisticsKPI Calculate(List<ScoreStatistic> allScores)
        {
            Dictionary<string, float> scores = new Dictionary<string, float>();

            UsedScores.ForEach(usedscore => scores.Add(
                usedscore,
                allScores.Where(score => score.ScoreName == usedscore).Select(score => score.ScoreValue).Sum()
            ));

            if (scores["PlateAppearence"] > 0)
            {
                float kpivalue = scores["Hit"] / scores["PlateAppearence"];
                return new StatisticsKPI(KPIName, kpivalue, allScores.First().PersonMLBId);
            }

            return new StatisticsKPI(KPIName, 1, allScores.First().PersonMLBId);
        }
    }

    public class RekenKPI_OBP : RekenKPI
    {
        public RekenKPI_OBP(string name) : base(name)
        {
            UsedScores = new List<string>()
            {
                "Hit",
                "BaseOnBalls",
                "IntentionalWalk",
                "HitByPitch",
                "PlateAppearence"
            };

        }

        public override StatisticsKPI Calculate(List<ScoreStatistic> allScores)
        {
            Dictionary<string, float> scores = new Dictionary<string, float>();

            UsedScores.ForEach(usedscore => scores.Add(
                usedscore,
                allScores.Where(score => score.ScoreName == usedscore).Select(score => score.ScoreValue).Sum()
            ));

            if (scores["PlateAppearence"] > 0)
            {
                                 //Hits         +                          Walks                      + HitByPitch            / PlateAppearances
                float kpivalue = (scores["Hit"] + (scores["IntentionalWalk"] + scores["BaseOnBalls"]) + scores["HitByPitch"]) / scores["PlateAppearence"];
                return new StatisticsKPI(KPIName, kpivalue, allScores.First().PersonMLBId);
            }

            return new StatisticsKPI(KPIName, 1, allScores.First().PersonMLBId);
        }
    }

    public class RekenKPI_SLG : RekenKPI
    {
        public RekenKPI_SLG(string name) : base(name)
        {
            UsedScores = new List<string>()
            {
                "Single",
                "Double",
                "Triple",
                "HomeRun",
                "PlateAppearence"
            };
        }

        public override StatisticsKPI Calculate(List<ScoreStatistic> allScores)
        {
            Dictionary<string, float> scores = new Dictionary<string, float>();

            UsedScores.ForEach(usedscore => scores.Add(
                usedscore,
                allScores.Where(score => score.ScoreName == usedscore).Select(score => score.ScoreValue).Sum()
            ));

            if (scores["PlateAppearence"] > 0)
            {
                float kpivalue = (scores["Single"] + (scores["Double"] * 2) + (scores["Triple"] * 3) + (scores["HomeRun"] * 4)) / scores["PlateAppearence"];
                return new StatisticsKPI(KPIName, kpivalue, allScores.First().PersonMLBId);
            }

            return new StatisticsKPI(KPIName, 1, allScores.First().PersonMLBId);
        }
    }

    public class RekenKPI_OPS : RekenKPI
    {
        public RekenKPI_OPS(string name) : base(name)
        {
            UsedScores = new List<string>()
            {
                "OBP",
                "SLG"
            };
        }

        public override StatisticsKPI Calculate(List<ScoreStatistic> allScores)
        {
            Dictionary<string, float> scores = new Dictionary<string, float>();

            float kpivalue = new RekenKPI_OBP("OBP").Calculate(allScores).StatisticsValue + new RekenKPI_SLG("SLG").Calculate(allScores).StatisticsValue;
            return new StatisticsKPI(KPIName, kpivalue, allScores.First().PersonMLBId);
        }
    }
}
