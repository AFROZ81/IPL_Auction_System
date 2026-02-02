using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPLAuctionSystem.Models;

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
    public bool IsSold { get; set; } = false;

    public string? ProfilePicture { get; set; }

    // NOT stored in Database (Used only for the upload form)
    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    // Foreign Key for Team
    public int? TeamId { get; set; }
    public Team? Team { get; set; }
}