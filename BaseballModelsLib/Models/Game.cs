using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseballModelsLib.Models
{
    public class Game
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string User{ get; set; }

        [DisplayFormat(NullDisplayText = "N/A")]
        [Required]
        public int HomeTeamId { get; set; }

        [DisplayFormat(NullDisplayText = "N/A")]
        [Required]
        public int AwayTeamId { get; set; }

        public String GameDate { get; set; } // YYYY-MM-DD
        public String GameTime { get; set; } // HH:MM

        [DisplayFormat(NullDisplayText = "N/A")]
        [Required]
        public int HomeStartingPitcherId { get; set; }

        [DisplayFormat(NullDisplayText = "N/A")]
        [Required]
        public int AwayStartingPitcherId { get; set; }

        public int TotalInnings { get; set; }

        // when game is finished fill:
        public bool Finished { get; set; }
        public int RunsHomeTeam { get; set; }
        public int RunsAwayTeam { get; set; }
        public int HitsHomeTeam { get; set; }
        public int HitsAwayTeam { get; set; }
        public int ErrorsHomeTeam { get; set; }
        public int ErrorsAwayTeam { get; set; }

        public Game()
        {
            TotalInnings = 9;
            Finished = false;
        }
    }
}
