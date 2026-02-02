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
    public class TeamsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public TeamsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Teams
        public async Task<IActionResult> Index()
        {
            return View(await _context.Teams.ToListAsync());
        }

        // GET: Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var team = await _context.Teams
                .Include(t => t.Players) // Include players to show roster in details
                .FirstOrDefaultAsync(m => m.Id == id);

            if (team == null) return NotFound();

            return View(team);
        }

        // GET: Teams/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Owner,Budget,LogoFile")] Team team)
        {
            if (ModelState.IsValid)
            {
                // Handle Logo Upload
                if (team.LogoFile != null)
                {
                    team.TeamLogo = await SaveImage(team.LogoFile);
                }

                _context.Add(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var team = await _context.Teams.FindAsync(id);
            if (team == null) return NotFound();

            return View(team);
        }

        // POST: Teams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Owner,Budget,TeamLogo,LogoFile")] Team team)
        {
            if (id != team.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // If a new file is uploaded
                    if (team.LogoFile != null)
                    {
                        // Delete old logo if it exists
                        if (!string.IsNullOrEmpty(team.TeamLogo))
                        {
                            DeleteOldImage(team.TeamLogo);
                        }

                        // Save new logo
                        team.TeamLogo = await SaveImage(team.LogoFile);
                    }

                    _context.Update(team);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(team.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var team = await _context.Teams
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null) return NotFound();

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team != null)
            {
                // Clean up the image file from the server
                if (!string.IsNullOrEmpty(team.TeamLogo))
                {
                    DeleteOldImage(team.TeamLogo);
                }

                _context.Teams.Remove(team);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // --- Helper Methods for File Management ---

        private async Task<string> SaveImage(IFormFile imageFile)
        {
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
            string path = Path.Combine(wwwRootPath + "/images/teams/", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return fileName;
        }

        private void DeleteOldImage(string fileName)
        {
            string filePath = Path.Combine(_hostEnvironment.WebRootPath + "/images/teams/", fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }
    }
}