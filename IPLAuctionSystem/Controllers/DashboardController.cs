using IPLAuctionSystem.Application.DTOs;
using IPLAuctionSystem.Application.Interfaces;
using IPLAuctionSystem.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IPLAuctionSystem.Controllers
{
    [Authorize(Policy = Policies.CanViewFinancials)]
    public class DashboardController : Controller
    {
        private readonly ITeamService _teamService;
        private readonly IPlayerService _playerService;

        public DashboardController(ITeamService teamService, IPlayerService playerService)
        {
            _teamService = teamService;
            _playerService = playerService;
        }

        public async Task<IActionResult> Index()
        {
            var teams = await _teamService.GetAllTeamsAsync();
            var players = await _playerService.GetAllPlayersAsync();

            var topPlayer = players.OrderByDescending(p => p.SoldPrice).FirstOrDefault();
            ViewBag.TopPlayer = topPlayer;

            return View(teams);
        }
    }
}
