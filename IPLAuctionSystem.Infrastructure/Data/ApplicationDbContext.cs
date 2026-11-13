namespace IPLAuctionSystem.Infrastructure.Data;

using IPLAuctionSystem.Domain.Entities;
using IPLAuctionSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Team> Teams { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<AuctionSession> AuctionSessions { get; set; }
    public DbSet<Bid> Bids { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Team configuration
        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Owner).HasMaxLength(200);
            entity.Property(t => t.Budget).HasColumnType("decimal(18,2)");
            entity.Property(t => t.TeamLogo).HasMaxLength(500);
            entity.HasMany(t => t.Players)
                .WithOne(p => p.Team)
                .HasForeignKey(p => p.TeamId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Player configuration
        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Category).HasConversion<string>().HasMaxLength(50);
            entity.Property(p => p.BasePrice).HasColumnType("decimal(18,2)");
            entity.Property(p => p.SoldPrice).HasColumnType("decimal(18,2)");
            entity.Property(p => p.AuctionStatus).HasConversion<int>();
            entity.Property(p => p.ProfilePicture).HasMaxLength(500);
            entity.HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // AuctionSession configuration
        modelBuilder.Entity<AuctionSession>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Name).IsRequired().HasMaxLength(200);
            entity.Property(a => a.Status).HasConversion<int>();
            entity.Property(a => a.RoundNumber).HasDefaultValue(1);
            entity.Property(a => a.StartedAt).HasColumnType("datetime2");
            entity.Property(a => a.CompletedAt).HasColumnType("datetime2");
            entity.HasMany(a => a.Bids)
                .WithOne(b => b.AuctionSession)
                .HasForeignKey(b => b.AuctionSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Bid configuration
        modelBuilder.Entity<Bid>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.Amount).HasColumnType("decimal(18,2)");
            entity.Property(b => b.BidTime).HasColumnType("datetime2");
            entity.HasOne(b => b.AuctionSession)
                .WithMany(a => a.Bids)
                .HasForeignKey(b => b.AuctionSessionId);
            entity.HasOne(b => b.Player)
                .WithMany()
                .HasForeignKey(b => b.PlayerId);
            entity.HasOne(b => b.Team)
                .WithMany()
                .HasForeignKey(b => b.TeamId);
        });
    }
}
