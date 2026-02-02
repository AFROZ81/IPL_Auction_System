using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IPLAuctionSystem.Data; // Ensure this matches your project's data namespace
using IPLAuctionSystem.Models;

namespace IPLAuctionSystem.Controllers
{
    public class HomeController : Controller
    {
        // 1. Declare the private field
        private readonly ApplicationDbContext _context;

        // 2. Inject the context via the Constructor
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Fetch the players for the main feed/top picks
            var players = await _context.Players
                .Include(p => p.Team)
                .ToListAsync();

            // 2. Fetch the teams for the Purse Tracker (This is what was missing!)
            ViewBag.Teams = await _context.Teams.ToListAsync();

            return View(players);
        }

        // ... Keep your other Privacy or Error actions below ...
    }
}