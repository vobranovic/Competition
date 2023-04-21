using System.ComponentModel.DataAnnotations.Schema;

namespace Competition.Models
{
    public class Match
    {
        public int Id { get; set; }
        public int ScoreTeamOne { get; set; } = 0;
        public int ScoreTeamTwo { get; set; } = 0;
        public int Round { get; set; }
        public DateTime MatchTime { get; set; }
        public bool Finished { get; set; }

        [NotMapped]
        public string? ClubName1 { get; set; }
        [NotMapped]
        public string? ClubName2 { get; set; }

        public int LeagueId { get; set; }

        [ForeignKey("MatchId")]
        public List<MatchClub> MatchClub { get; set; }  = new List<MatchClub>();
    }
}
