namespace IPLAuctionSystem.Application.DTOs;

public class BidDto
{
    public int Id { get; set; }
    public int AuctionSessionId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public string? TeamName { get; set; }
    public decimal Amount { get; set; }
    public DateTime BidTime { get; set; }
    public bool IsWinningBid { get; set; }
}

public class PlaceBidDto
{
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public decimal Amount { get; set; }
}

public class AuctionSessionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int CurrentPlayerId { get; set; }
    public string? CurrentPlayerName { get; set; }
    public int RoundNumber { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public List<BidDto> RecentBids { get; set; } = new();
}
