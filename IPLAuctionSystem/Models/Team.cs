using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace IPLAuctionSystem.Models;

public class Team
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
    public string? Owner { get; set; }

    [Precision(18, 2)]
    public decimal? Budget { get; set; } = 100000000; // 100 Crores
    public string? TeamLogo { get; set; }

    // Not saved in Database - used for the form upload
    [NotMapped]
    public IFormFile? LogoFile { get; set; }

    // Relationship: One Team has many Players
    public ICollection<Player>? Players { get; set; }
}