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
            // Now '_context' will work!
            var recentSignings = await _context.Players
                .Include(p => p.Team)
                .Where(p => p.TeamId != null)
                .OrderByDescending(p => p.Id)
                .Take(5)
                .ToListAsync();

            return View(recentSignings);
        }

        // ... Keep your other Privacy or Error actions below ...
    }
}