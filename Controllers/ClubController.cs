using Competition.Data;
using Competition.Models;
using Microsoft.AspNetCore.Mvc;

namespace Competition.Controllers
{
    public class ClubController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public ClubController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult EditClubs(int id)
        {
            if (_dbContext.Leagues.FirstOrDefault(l => l.Id == id).Started)
            {
                return RedirectToAction("View", "League", new { id });
            }

            var clubs = GetClubsInLeague(id);
            ViewBag.LeagueId = id;
            return View(clubs);
        }

        [HttpGet]
        public IActionResult Add(int id)
        {
            if (_dbContext.Leagues.FirstOrDefault(l => l.Id == id).Started)
            {
                return RedirectToAction("View", "League", new { id });
            }
            ViewBag.LeagueId = id;
            return View();
        }

        [HttpPost]
        public IActionResult Add([Bind("Name")]Club club, int id)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Clubs.Add(club);
                _dbContext.SaveChanges();

                _dbContext.LeagueClub.Add(new LeagueClub() { ClubId = club.Id, LeagueId = id });
                _dbContext.SaveChanges();

                return RedirectToAction("EditClubs", new { id });
            }

            return View(club);
        }

        public IActionResult Remove(int id)
        {
            var club = _dbContext.Clubs.Find(id);
            var lc = _dbContext.LeagueClub.FirstOrDefault(lc => lc.ClubId == id);
            _dbContext.Clubs.Remove(club);
            _dbContext.SaveChanges();

            return RedirectToAction("EditClubs", new { id = lc.LeagueId });

        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var club = _dbContext.Clubs.Find(id);
            var lc = _dbContext.LeagueClub.FirstOrDefault(lc => lc.ClubId == id);
            ViewBag.LeagueId = lc.LeagueId;

            return View(club);
        }

        [HttpPost]
        public IActionResult Edit(Club club, int id)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Clubs.Update(club);
                _dbContext.SaveChanges();
                var lc = _dbContext.LeagueClub.FirstOrDefault(lc => lc.ClubId == id);

                return RedirectToAction("EditClubs", new { id = lc.LeagueId });
            }

            return View(club);
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
    }
}
