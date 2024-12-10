using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseballModelsLib.Models
{
    public class Team
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [DisplayName("Ploegnaam")]
        [DisplayFormat(NullDisplayText = "FA")]
        [Required]
        public string Name { get; set; }

        [DisplayName("Aantal Spelers")]
        public int TotalPlayers { get; set; } = 0;

        public string? MLB_Org_ID { get; set; } = null;
        public string? VenueName { get; set; } = null;

        public string? LeagueName { get; set; } = null;
        public string? NameDisplayBrief { get; set; } = null;
        public string? City { get; set; } = null;
        public string? FranchiseCode { get; set; } = null;

        public bool Deleted { get; set; } = false;
        public Team()
        {
            Name = "";
        }
    }
}
