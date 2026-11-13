using IPLAuctionSystem.Application.DTOs;
using IPLAuctionSystem.Application.Interfaces;
using IPLAuctionSystem.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IPLAuctionSystem.Controllers
{
    [Authorize]
    public class AuctionController : Controller
    {
        private readonly IAuctionService _auctionService;
        private readonly IBidService _bidService;
        private readonly ITeamService _teamService;
        private readonly IPlayerService _playerService;

        public AuctionController(IAuctionService auctionService, IBidService bidService, ITeamService teamService, IPlayerService playerService)
        {
            _auctionService = auctionService;
            _bidService = bidService;
            _teamService = teamService;
            _playerService = playerService;
        }

        // GET: Auction War Room
        [Authorize(Policy = Policies.CanViewFinancials)]
        public async Task<IActionResult> Index()
        {
            var currentAuction = await _auctionService.GetCurrentAuctionAsync();
            var teams = await _teamService.GetAllTeamsAsync();

            ViewBag.Teams = teams;
            return View(currentAuction);
        }

        // POST: Finalize Sale
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Policies.CanRunAuction)]
        public async Task<IActionResult> SellPlayer(int playerId, int teamId, decimal finalPrice)
        {
            var success = await _playerService.SellPlayerAsync(playerId, teamId, finalPrice);

            if (!success)
            {
                TempData["Error"] = "Insufficient budget or invalid player/team.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Player sold!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Mark Unsold
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Policies.CanRunAuction)]
        public async Task<IActionResult> MarkUnsold(int playerId)
        {
            var success = await _playerService.MarkPlayerUnsoldAsync(playerId);

            if (success)
            {
                TempData["Info"] = "Player marked as unsold.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
