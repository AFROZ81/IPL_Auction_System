using IPLAuctionSystem.Application.DTOs;
using IPLAuctionSystem.Application.Interfaces;
using IPLAuctionSystem.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IPLAuctionSystem.Controllers
{
    [Authorize]
    public class TeamsController : Controller
    {
        private readonly ITeamService _teamService;

        public TeamsController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        // GET: Teams
        [Authorize(Policy = Policies.CanViewFinancials)]
        public async Task<IActionResult> Index()
        {
            var teams = await _teamService.GetAllTeamsAsync();
            return View(teams);
        }

        // GET: Teams/Details/5
        [Authorize(Policy = Policies.CanViewFinancials)]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var team = await _teamService.GetTeamByIdAsync(id.Value);
            if (team == null) return NotFound();

            return View(team);
        }

        // GET: Teams/Create
        [Authorize(Roles = $"{Roles.Admin},{Roles.Auctioneer}")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Auctioneer}")]
        public async Task<IActionResult> Create(CreateTeamDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _teamService.CreateTeamAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // GET: Teams/Edit/5
        [Authorize(Roles = $"{Roles.Admin},{Roles.Auctioneer}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var team = await _teamService.GetTeamByIdAsync(id.Value);
            if (team == null) return NotFound();

            var dto = new UpdateTeamDto
            {
                Id = team.Id,
                Name = team.Name,
                Owner = team.Owner,
                Budget = team.Budget
            };

            return View(dto);
        }

        // POST: Teams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Admin},{Roles.Auctioneer}")]
        public async Task<IActionResult> Edit(int id, UpdateTeamDto dto)
        {
            if (id != dto.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _teamService.UpdateTeamAsync(id, dto);
            return RedirectToAction(nameof(Index));
        }

        // GET: Teams/Delete/5
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var team = await _teamService.GetTeamByIdAsync(id.Value);
            if (team == null) return NotFound();

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _teamService.DeleteTeamAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
