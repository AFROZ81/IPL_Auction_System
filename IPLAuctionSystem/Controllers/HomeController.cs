using IPLAuctionSystem.Application.DTOs;
using IPLAuctionSystem.Application.Interfaces;
using IPLAuctionSystem.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IPLAuctionSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ITeamService _teamService;
        private readonly IPlayerService _playerService;

        public HomeController(ITeamService teamService, IPlayerService playerService)
        {
            _teamService = teamService;
            _playerService = playerService;
        }

        public async Task<IActionResult> Index()
        {
            var players = await _playerService.GetAllPlayersAsync();
            var teams = await _teamService.GetAllTeamsAsync();

            ViewBag.Teams = teams;
            return View(players);
        }
    }
}
