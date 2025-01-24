using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BaseballModelsLib.Models
{
    public class Player
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        
        [DisplayName("Naam")]
        [Required]
        public string Name { get; set; }

        [DisplayName("Rugnummer")]
        [DisplayFormat(NullDisplayText = "N/A")]
        public int? Rugnummer { get; set; } = null;

        [DisplayName("Positie")]
        [DisplayFormat(NullDisplayText = "N/A")]
        public string? Position { get; set; } = null;

        [DisplayName("Geboortedatum")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        public string? APILink { get; set; } = null;
        public int? MLBPersonId { get; set; } = null;

        public int? TeamId { get; set; } = null;

        public bool Deleted { get; set; } = false;

        [JsonIgnore]
        [DisplayName("Ploeg")]
        [NotMapped]
        public Team? Team { get; set; }

        public Player()
        {
            Name = "";
        }

    }
}
