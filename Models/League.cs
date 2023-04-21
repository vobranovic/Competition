using System.ComponentModel.DataAnnotations.Schema;

namespace Competition.Models
{
    public class League
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Started { get; set; }
        public bool HomeAndAway { get; set; } 

        [ForeignKey("LeagueId")]
        public List<Match> Matches { get; set; } = new List<Match>();

        [ForeignKey("LeagueId")]
        public List<LeagueClub> LeagueClub { get; set; } = new List<LeagueClub>();
    }
}
