using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IPLAuctionSystem.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IPLAuctionSystem.Controllers
{
    public class AuctionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuctionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Auction War Room
        public async Task<IActionResult> Index()
        {
            // Get the first player who isn't processed yet
            var player = await _context.Players
                .OrderBy(p => p.Id)
                .FirstOrDefaultAsync(p => !p.IsSold);

            // Get teams and create a SelectList for the dropdown
            var teamsList = await _context.Teams.ToListAsync();
            ViewBag.Teams = new SelectList(teamsList, "Id", "Name");

            return View(player);
        }

        // POST: Finalize Sale
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SellPlayer(int playerId, int teamId, decimal finalPrice)
        {
            var player = await _context.Players.FindAsync(playerId);
            var team = await _context.Teams.FindAsync(teamId);

            if (player == null || team == null) return RedirectToAction(nameof(Index));

            if (team.Budget < finalPrice)
            {
                TempData["Error"] = "Insufficient budget for this team!";
                return RedirectToAction(nameof(Index));
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                team.Budget -= finalPrice;
                player.IsSold = true;
                player.TeamId = teamId;
                player.SoldPrice = finalPrice;

                _context.Update(team);
                _context.Update(player);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["Success"] = $"{player.Name} SOLD to {team.Name}!";
            }
            catch { await transaction.RollbackAsync(); }

            return RedirectToAction(nameof(Index));
        }

        // POST: Mark Unsold (Pass)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkUnsold(int playerId)
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player != null)
            {
                player.IsSold = true; // Mark as processed
                player.SoldPrice = 0; // No price
                player.TeamId = null; // No team
                _context.Update(player);
                await _context.SaveChangesAsync();
                TempData["Info"] = $"{player.Name} went unsold.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}