using IPLAuctionSystem.Application.DTOs;
using IPLAuctionSystem.Application.Interfaces;
using IPLAuctionSystem.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IPLAuctionSystem.Controllers
{
    [Authorize]
    public class PlayersController : Controller
    {
        private readonly IPlayerService _playerService;
        private readonly ITeamService _teamService;

        public PlayersController(IPlayerService playerService, ITeamService teamService)
        {
            _playerService = playerService;
            _teamService = teamService;
        }

        // GET: Players
        [Authorize(Policy = Policies.CanViewFinancials)]
        public async Task<IActionResult> Index()
        {
            var players = await _playerService.GetAllPlayersAsync();
            var teams = await _teamService.GetAllTeamsAsync();

            ViewBag.Teams = teams;
            return View(players);
        }

        // POST: Players/SellPlayer
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = Policies.CanRunAuction)]
        public async Task<IActionResult> SellPlayer(int playerId, int teamId, decimal bidAmount)
        {
            var success = await _playerService.SellPlayerAsync(playerId, teamId, bidAmount);

            if (!success)
            {
                TempData["Error"] = "Unable to complete the sale. Check funds and player/team validity.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Player sold successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Players/Details/5
        [Authorize(Policy = Policies.CanViewFinancials)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var player = await _playerService.GetPlayerByIdAsync(id.Value);
            if (player == null) return NotFound();

            return View(player);
        }

        // GET: Players/Create
        [Authorize(Roles = $"{Roles.Admin},{Roles.FranchiseOwner}")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Players/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Admin},{Roles.FranchiseOwner}")]
        public async Task<IActionResult> Create(CreatePlayerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _playerService.CreatePlayerAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // GET: Players/Edit/5
        [Authorize(Roles = $"{Roles.Admin},{Roles.FranchiseOwner}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var player = await _playerService.GetPlayerByIdAsync(id.Value);
            if (player == null) return NotFound();

            var dto = new UpdatePlayerDto
            {
                Name = player.Name,
                Category = player.Category,
                BasePrice = player.BasePrice,
                SoldPrice = player.SoldPrice,
                AuctionStatus = player.AuctionStatus,
                TeamId = player.TeamId
            };

            return View(dto);
        }

        // POST: Players/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Admin},{Roles.FranchiseOwner}")]
        public async Task<IActionResult> Edit(int id, UpdatePlayerDto dto)
        {
            if (id != dto.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _playerService.UpdatePlayerAsync(id, dto);
            return RedirectToAction(nameof(Index));
        }

        // GET: Players/Delete/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var player = await _playerService.GetPlayerByIdAsync(id.Value);
            if (player == null) return NotFound();

            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _playerService.DeletePlayerAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
