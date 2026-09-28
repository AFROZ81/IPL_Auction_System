using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPLAuctionSystem.Models;

public enum AuctionStatus
{
    Pending = 0, // Still in the auction pool
    Sold = 1,    // Bought by a franchise
    Unsold = 2   // Passed in the war room, can be re-auctioned
}

public class Player
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    public string? Category { get; set; } // Batsman, Bowler, etc.

    [Precision(18, 2)]
    public decimal BasePrice { get; set; }

    [Precision(18, 2)]
    public decimal SoldPrice { get; set; }

    // Replaces the old ambiguous IsSold flag (which was true for BOTH sold and unsold players)
    public AuctionStatus AuctionStatus { get; set; } = AuctionStatus.Pending;

    public string? ProfilePicture { get; set; }

    // NOT stored in Database (Used only for the upload form)
    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    // Foreign Key for Team
    public int? TeamId { get; set; }
    public Team? Team { get; set; }
}