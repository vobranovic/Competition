using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Competition.Models
{
    public class Club
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int? Points { get; set; } = 0;
        public int? MatchesPlayed { get; set; } = 0;
        public int? Win { get; set; } = 0;
        public int? Draw { get; set; } = 0;
        public int? Loss { get; set; } = 0;
        public int? GoalsFor { get; set; } = 0;
        public int? GoalsAgainst { get; set; } = 0;



        [ForeignKey("ClubId")]
        public List<MatchClub> MatchClub { get; set; } = new List<MatchClub>();

        [ForeignKey("ClubId")]
        public List<LeagueClub> LeagueClub { get; set; } = new List<LeagueClub>();
    }
}
