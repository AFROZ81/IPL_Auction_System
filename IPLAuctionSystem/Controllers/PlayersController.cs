using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IPLAuctionSystem.Data;
using IPLAuctionSystem.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace IPLAuctionSystem.Controllers
{
    public class PlayersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PlayersController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Players
        public async Task<IActionResult> Index()
        {
            // Populate ViewBag with Teams so the "Sell" modal dropdown has data
            ViewBag.Teams = await _context.Teams.OrderBy(t => t.Name).ToListAsync();

            var applicationDbContext = _context.Players.Include(p => p.Team);
            return View(await applicationDbContext.ToListAsync());
        }

        // NEW ACTION: Sell Player Logic
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SellPlayer(int playerId, int teamId, decimal bidAmount)
        {
            var player = await _context.Players.FindAsync(playerId);
            var team = await _context.Teams.FindAsync(teamId);

            if (player == null || team == null)
            {
                TempData["Error"] = "Player or Franchise not found.";
                return RedirectToAction(nameof(Index));
            }

            // Financial Validation
            if (team.Budget < bidAmount)
            {
                TempData["Error"] = $"Insufficient Funds! {team.Name} only has ₹{team.Budget:N0} remaining.";
                return RedirectToAction(nameof(Index));
            }

            // Transactional update to ensure data integrity
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. Deduct from Team Budget
                    team.Budget -= bidAmount;

                    // 2. Update Player details
                    player.TeamId = teamId;
                    player.SoldPrice = bidAmount;
                    player.IsSold = true;

                    _context.Update(team);
                    _context.Update(player);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Success"] = $"HAMMER DOWN! {player.Name} sold to {team.Name} for ₹{bidAmount:N0}.";
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    TempData["Error"] = "Critical error during auction transaction.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Players/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var player = await _context.Players
                .Include(p => p.Team)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (player == null) return NotFound();

            return View(player);
        }

        // GET: Players/Create
        public IActionResult Create()
        {
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name");
            return View();
        }

        // POST: Players/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Category,BasePrice,SoldPrice,IsSold,TeamId,ImageFile")] Player player)
        {
            if (ModelState.IsValid)
            {
                if (player.ImageFile != null)
                {
                    player.ProfilePicture = await UploadImage(player.ImageFile);
                }

                _context.Add(player);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name", player.TeamId);
            return View(player);
        }

        // GET: Players/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var player = await _context.Players.FindAsync(id);
            if (player == null) return NotFound();

            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name", player.TeamId);
            return View(player);
        }

        // POST: Players/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Category,BasePrice,SoldPrice,IsSold,TeamId,ProfilePicture,ImageFile")] Player player)
        {
            if (id != player.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (player.ImageFile != null)
                    {
                        // Delete old image if it exists to save space
                        if (!string.IsNullOrEmpty(player.ProfilePicture))
                        {
                            string oldPath = Path.Combine(_hostEnvironment.WebRootPath, "images", "players", player.ProfilePicture);
                            if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                        }
                        player.ProfilePicture = await UploadImage(player.ImageFile);
                    }

                    _context.Update(player);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerExists(player.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name", player.TeamId);
            return View(player);
        }

        // GET: Players/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var player = await _context.Players
                .Include(p => p.Team)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (player == null) return NotFound();

            return View(player);
        }

        // POST: Players/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player != null)
            {
                // Delete physical image file
                if (!string.IsNullOrEmpty(player.ProfilePicture))
                {
                    string filePath = Path.Combine(_hostEnvironment.WebRootPath, "images", "players", player.ProfilePicture);
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                }
                _context.Players.Remove(player);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> UploadImage(IFormFile file)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string uploadPath = Path.Combine(wwwRootPath, "images", "players");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            string fullPath = Path.Combine(uploadPath, fileName);
            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return fileName;
        }

        private bool PlayerExists(int id)
        {
            return _context.Players.Any(e => e.Id == id);
        }
    }
}