using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseballModelsLib.Models
{
    public class Season
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [DisplayName("Speler")]
        [Required]
        public int PlayerKey { get; set; }

        [DisplayName("Games")]
        [Required]
        public int GamesPlayed { get; set; }

        [DisplayName("Jaar")]
        [Required]
        public int Year { get; set; }
        //Batting

        [DisplayName("PA")]
        public int PlateAppearences { get; set; }

        [DisplayName("K")]
        public int HStrikeOuts { get; set; }

        [DisplayName("H")]
        public int Hits { get; set; }

        [DisplayName("1B")]
        public int Singles { get; set; }

        [DisplayName("2B")]
        public int Doubles { get; set; }

        [DisplayName("3B")]
        public int Triples { get; set; }

        [DisplayName("HR")]
        public int HomeRuns { get; set; }

        [DisplayName("BB")]
        public int BaseOnBalls { get; set; }

        [DisplayName("HBP")]
        public int HitByPitch { get; set; }

        [DisplayName("SF")]
        public int SacrificeFlies { get; set; }

        [DisplayName("SH")]
        public int SacrificeHits { get; set; }

        //Running
        [DisplayName("CS")]
        public int CaughtStealing { get; set; }

        [DisplayName("SB")]
        public int StolenBases { get; set; }

        [DisplayName("R")]
        public int Runs { get; set; }

        //Fielding
        [DisplayName("E")]
        public int Errors { get; set; }

        [DisplayName("DP")]
        public int DoublePlays { get; set; }

        [DisplayName("TP")]
        public int TriplePlays { get; set; }

        [DisplayName("PB")]
        public int PassedBalls { get; set; }

        //Pitching
        [DisplayName("K")]
        public int PStrikeOuts { get; set; }

        public bool Deleted { get; set; } = false;
    }
}
