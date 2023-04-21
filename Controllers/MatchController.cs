using Competition.Data;
using Competition.Models;
using Microsoft.AspNetCore.Mvc;

namespace Competition.Controllers
{
    public class MatchController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public MatchController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var match = _dbContext.Matches.Find(id);

            var clubsInMatch = _dbContext.MatchClub.Where(mc => mc.MatchId == id).ToList();
            ViewBag.ClubName1 = _dbContext.Clubs.FirstOrDefault(c => c.Id == clubsInMatch[0].ClubId).Name;
            ViewBag.ClubName2 = _dbContext.Clubs.FirstOrDefault(c => c.Id == clubsInMatch[1].ClubId).Name;

            return View(match);
        }

        [HttpPost]
        public IActionResult Edit(Match match)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Matches.Update(match);
                _dbContext.SaveChanges();

                return RedirectToAction("View", "League", new { id = match.LeagueId });
            }

            return View(match);
        }
    }
}
