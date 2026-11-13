namespace IPLAuctionSystem.Application.DTOs;

public class PlayerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal SoldPrice { get; set; }
    public string AuctionStatus { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
    public int? TeamId { get; set; }
    public string? TeamName { get; set; }
}

public class CreatePlayerDto
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public string AuctionStatus { get; set; } = "Pending";
}

public class UpdatePlayerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal SoldPrice { get; set; }
    public string AuctionStatus { get; set; } = string.Empty;
    public int? TeamId { get; set; }
}
