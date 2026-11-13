namespace IPLAuctionSystem.Domain.Entities;

using IPLAuctionSystem.Domain.Common;
using IPLAuctionSystem.Domain.ValueObjects;

public class Bid : Entity
{
    public int AuctionSessionId { get; set; }
    public AuctionSession? AuctionSession { get; set; }
    public int PlayerId { get; set; }
    public Player? Player { get; set; }
    public int TeamId { get; set; }
    public Team? Team { get; set; }
    public Money Amount { get; set; } = new(0m, "INR");
    public DateTime BidTime { get; set; } = DateTime.UtcNow;
    public bool IsWinningBid { get; set; }

    private Bid() { } // EF Core

    public Bid(int auctionSessionId, int playerId, int teamId, Money amount)
    {
        AuctionSessionId = auctionSessionId;
        PlayerId = playerId;
        TeamId = teamId;
        Amount = amount;
    }

    public void MarkAsWinning()
    {
        IsWinningBid = true;
    }

    public void MarkAsNotWinning()
    {
        IsWinningBid = false;
    }
}
