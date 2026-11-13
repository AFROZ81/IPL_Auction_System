using IPLAuctionSystem.Application.Interfaces;
using IPLAuctionSystem.Domain.Constants;
using IPLAuctionSystem.Hubs;
using IPLAuctionSystem.Infrastructure.Data;
using IPLAuctionSystem.Infrastructure.Repositories;
using IPLAuctionSystem.Infrastructure.Services;
using IPLAuctionSystem.Infrastructure.Services.Implementations;
using IPLAuctionSystem.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/ipc-auction-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/ipc-auction-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7));

    builder.Services.AddControllersWithViews();

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null)));

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 12;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    var jwtKey = builder.Configuration["JwtSettings:SecretKey"] ?? "IPLAuctionSystemSecretKey2026!@#$%";
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "IPLAuctionSystem",
            ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? "IPLAuctionSystemUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy(Policies.CanRunAuction, policy =>
            policy.RequireRole(Roles.Admin, Roles.Auctioneer));

        options.AddPolicy(Policies.CanViewFinancials, policy =>
            policy.RequireRole(Roles.Admin, Roles.Auctioneer, Roles.FranchiseOwner));

        options.AddPolicy(Policies.CanEditOwnTeam, policy =>
            policy.RequireAuthenticatedUser());
    });

    builder.Services.AddSignalR();

    builder.Services.AddHealthChecks();

    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/javascript", "text/css", "text/html", "text/json", "text/xml" });
    });

    builder.Services.AddAutoMapper(typeof(MappingProfile));

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<ITeamService, TeamService>();
    builder.Services.AddScoped<IPlayerService, PlayerService>();
    builder.Services.AddScoped<IAuctionService, AuctionService>();
    builder.Services.AddScoped<IBidService, BidService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<IFileStorage, LocalFileStorage>();
    builder.Services.AddScoped<RoleSeeder>();

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<RoleSeeder>();
        await seeder.SeedRolesAsync();
        await seeder.SeedSuperAdminAsync();
    }

    app.UseSerilogRequestLogging();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseResponseCompression();

    app.Use(async (context, next) =>
    {
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "SAMEORIGIN");
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https://assets.mixkit.co https://cdnjs.cloudflare.com; font-src 'self' https://cdnjs.cloudflare.com; connect-src 'self'");
        await next();
    });

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapStaticAssets();

    app.MapHealthChecks("/health");

    app.MapHub<AuctionHub>("/auctionHub");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
