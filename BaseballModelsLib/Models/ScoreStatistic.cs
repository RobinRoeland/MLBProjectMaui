using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace BaseballModelsLib.Models
{
    public class ScoreStatistic
    {
        // Fields converted to properties
        public string ScoreName { get; set; } = "";
        public float ScoreValue { get; set; } = 0.0f;
        public string DefensivePlay { get; set; } = "";

        // Primary key properties
        [Required]
        public int GameId { get; set; }

        [Required]
        public int Inning { get; set; }

        [Required]
        public int InningTop { get; set; }

        [Required]
        public int PersonMLBId { get; set; }

        [Required]
        public int ScoreIdInInning { get; set; }

        public ScoreStatistic()
        {
            ScoreName = "";
            ScoreValue = 0.0f;
            DefensivePlay = "";
            GameId = 0;
            Inning = 0;
            InningTop = 1; // 0 is top, 1 is bottom
            ScoreIdInInning = 0;
        }
    }
}
