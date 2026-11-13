namespace IPLAuctionSystem.Infrastructure.Services.Implementations;

using IPLAuctionSystem.Application.Interfaces;
using IPLAuctionSystem.Domain.Constants;
using IPLAuctionSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class RoleSeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoleSeeder(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task SeedRolesAsync()
    {
        var roles = new[] { Roles.Admin, Roles.Auctioneer, Roles.FranchiseOwner, Roles.Viewer };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public async Task SeedSuperAdminAsync()
    {
        var adminEmail = "admin@iplauction.com";
        var adminUser = await _userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Administrator",
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(adminUser, "Admin@123456");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, Roles.Admin);
            }
        }
    }
}
