using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IPLAuctionSystem.Data;

namespace IPLAuctionSystem.Controllers;

public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var teams = await _context.Teams.Include(t => t.Players).ToListAsync();
        var topPlayer = await _context.Players.OrderByDescending(p => p.SoldPrice).FirstOrDefaultAsync();

        ViewBag.TopPlayer = topPlayer;
        return View(teams);
    }
}