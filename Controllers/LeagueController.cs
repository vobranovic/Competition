using Competition.Data;
using Competition.Models;
using Microsoft.AspNetCore.Mvc;

namespace Competition.Controllers
{
    public class LeagueController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public LeagueController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var leagues = _dbContext.Leagues.ToList();
            return View(leagues);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(League league)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Leagues.Add(league);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(league);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var league = _dbContext.Leagues.Find(id);
            _dbContext.Leagues.Remove(league);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Start(int id)
        {
            var league = _dbContext.Leagues.Find(id);

            var clubs = GetClubsInLeague(id);
            if(clubs.Count % 2 != 0 || clubs.Count <= 2)
            {
                TempData["NumberOfClubsError"] = "To start a league, number of clubs must be more than 2 and the number of clubs in the league cannot be an odd number!";
                return RedirectToAction("Index");
            }
            var shuffledClubs = ShuffleClubs(clubs);
            ScheduleRoundRobin(shuffledClubs, id);

            league.Started = true;
            _dbContext.Leagues.Update(league);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult View(int id)
        {
            var matchesInLeague = _dbContext.Matches.Where(m => m.LeagueId == id).ToList();

            foreach (var item in matchesInLeague)
            {
                var clubsInMatch = _dbContext.MatchClub.Where(mc => mc.MatchId == item.Id).ToList();
                item.ClubName1 = _dbContext.Clubs.FirstOrDefault(c => c.Id == clubsInMatch[0].ClubId).Name;
                item.ClubName2 = _dbContext.Clubs.FirstOrDefault(c => c.Id == clubsInMatch[1].ClubId).Name;
            }

            if (_dbContext.Leagues.Find(id).HomeAndAway)
            {
                ViewBag.NumberOfRounds = (_dbContext.LeagueClub.Count(lc => lc.LeagueId == id) * 2) - 1;
            }
            else
            {
                ViewBag.NumberOfRounds = _dbContext.LeagueClub.Count(lc => lc.LeagueId == id);
            }

            var clubsInLeague = GetClubsInLeague(id);
            var finishedMatchesInLeague = _dbContext.Matches.Where(m => m.LeagueId == id && m.Finished == true).ToList();

            foreach (var item in finishedMatchesInLeague)
            {
                var clubsInMatch = _dbContext.MatchClub.Where(i => i.MatchId == item.Id).ToList();
                var club1Id = clubsInMatch[0].ClubId;
                var club2Id = clubsInMatch[1].ClubId;

                if (item.ScoreTeamOne > item.ScoreTeamTwo)
                {
                    clubsInLeague.FirstOrDefault(c => c.Id == club1Id).Points += 3;
                    clubsInLeague.FirstOrDefault(c => c.Id == club1Id).Win += 1;
                    clubsInLeague.FirstOrDefault(c => c.Id == club1Id).MatchesPlayed += 1;

                    clubsInLeague.FirstOrDefault(c => c.Id == club2Id).Loss += 1;
                    clubsInLeague.FirstOrDefault(c => c.Id == club2Id).MatchesPlayed += 1;
                }
                else if (item.ScoreTeamOne < item.ScoreTeamTwo)
                {
                    clubsInLeague.FirstOrDefault(c => c.Id == club2Id).Points += 3;
                    clubsInLeague.FirstOrDefault(c => c.Id == club2Id).Win += 1;
                    clubsInLeague.FirstOrDefault(c => c.Id == club2Id).MatchesPlayed += 1;

                    clubsInLeague.FirstOrDefault(c => c.Id == club1Id).Loss += 1;
                    clubsInLeague.FirstOrDefault(c => c.Id == club1Id).MatchesPlayed += 1;
                }

                else if (item.ScoreTeamOne == item.ScoreTeamTwo)
                {
                    clubsInLeague.FirstOrDefault(c => c.Id == club1Id).Points += 1;
                    clubsInLeague.FirstOrDefault(c => c.Id == club1Id).Draw += 1;
                    clubsInLeague.FirstOrDefault(c => c.Id == club1Id).MatchesPlayed += 1;

                    clubsInLeague.FirstOrDefault(c => c.Id == club2Id).Points += 1;
                    clubsInLeague.FirstOrDefault(c => c.Id == club2Id).Draw += 1;
                    clubsInLeague.FirstOrDefault(c => c.Id == club2Id).MatchesPlayed += 1;
                }
                
            }
            var clubsInLeagueSorted = clubsInLeague.OrderByDescending(c => c.Points);

            ViewBag.ClubsInLeague = clubsInLeagueSorted;
            return View(matchesInLeague);
        }

        


        private List<Club> GetClubsInLeague(int leagueId)
        {
            var clubsInLeague = _dbContext.LeagueClub.Where(lc => lc.LeagueId == leagueId).ToList();

            List<Club> clubs = new List<Club>();

            foreach (var club in clubsInLeague)
            {
                clubs.Add(_dbContext.Clubs.FirstOrDefault(c => c.Id == club.ClubId));
            }

            return clubs;
        }

        private void ScheduleRoundRobin(List<Club> clubs, int leagueId)
        {
            var clubGroup1 = clubs.Take(clubs.Count / 2).ToList();
            var clubGroup2 = clubs.TakeLast(clubs.Count / 2).ToList();

            for (int i = 1; i < clubs.Count; i++)
            {
                for (int j = 0; j < clubs.Count / 2; j++)
                {
                    Match match = new Match() { LeagueId = leagueId, Round = i, MatchTime = DateTime.Today };
                    _dbContext.Matches.Add(match);
                    _dbContext.SaveChanges();

                    _dbContext.MatchClub.Add(new MatchClub() { MatchId = _dbContext.Matches.OrderBy(m => m.Id).Last(m => m.LeagueId == leagueId).Id, ClubId = clubGroup1[j].Id });
                    _dbContext.SaveChanges();
                    _dbContext.MatchClub.Add(new MatchClub() { MatchId = _dbContext.Matches.OrderBy(m => m.Id).Last(m => m.LeagueId == leagueId).Id, ClubId = clubGroup2[j].Id });
                    _dbContext.SaveChanges();
                }

                var clubToMove1 = clubGroup1.Last();
                var clubToMove2 = clubGroup2.First();

                clubGroup1.RemoveAt(clubGroup1.Count - 1);
                clubGroup2.RemoveAt(0);

                clubGroup1.Insert(1, clubToMove2);
                clubGroup2.Add(clubToMove1);
                
            }

            if (_dbContext.Leagues.Find(leagueId).HomeAndAway)
            {
                for (int i = clubs.Count; i < (clubs.Count * 2) - 1; i++)
                {
                    for (int j = 0; j < clubs.Count / 2; j++)
                    {
                        Match match = new Match() { LeagueId = leagueId, Round = i, MatchTime = DateTime.Today };
                        _dbContext.Matches.Add(match);
                        _dbContext.SaveChanges();

                        _dbContext.MatchClub.Add(new MatchClub() { MatchId = _dbContext.Matches.OrderBy(m => m.Id).Last(m => m.LeagueId == leagueId).Id, ClubId = clubGroup2[j].Id });
                        _dbContext.SaveChanges();
                        _dbContext.MatchClub.Add(new MatchClub() { MatchId = _dbContext.Matches.OrderBy(m => m.Id).Last(m => m.LeagueId == leagueId).Id, ClubId = clubGroup1[j].Id });
                        _dbContext.SaveChanges();
                    }

                    var clubToMove1 = clubGroup1.Last();
                    var clubToMove2 = clubGroup2.First();

                    clubGroup1.RemoveAt(clubGroup1.Count - 1);
                    clubGroup2.RemoveAt(0);

                    clubGroup1.Insert(1, clubToMove2);
                    clubGroup2.Add(clubToMove1);
                }
            }
        }

        private List<Club> ShuffleClubs(List<Club> clubs)
        {
            List<Club> shuffledClubs = new List<Club>();

            while (clubs.Count > 0)
            {
                Random random = new Random();
                int element = random.Next(0, clubs.Count - 1);
                shuffledClubs.Add(clubs.ElementAt(element));
                clubs.RemoveAt(element);
            }

            return shuffledClubs;
        }
    }
}
