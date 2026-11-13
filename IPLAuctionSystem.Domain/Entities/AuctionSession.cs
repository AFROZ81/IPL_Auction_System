namespace IPLAuctionSystem.Domain.Entities;

using IPLAuctionSystem.Domain.Common;
using IPLAuctionSystem.Domain.Enums;
using IPLAuctionSystem.Domain.ValueObjects;

public class AuctionSession : Entity
{
    public string Name { get; set; } = string.Empty;
    public AuctionSessionStatus Status { get; set; } = AuctionSessionStatus.Scheduled;
    public int CurrentPlayerId { get; set; }
    public Player? CurrentPlayer { get; set; }
    public int RoundNumber { get; set; } = 1;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public List<Bid> Bids { get; set; } = new();

    private AuctionSession() { } // EF Core

    public AuctionSession(string name, int roundNumber = 1)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        RoundNumber = roundNumber;
        StartedAt = DateTime.UtcNow;
    }

    public void Start()
    {
        Status = AuctionSessionStatus.Active;
    }

    public void Pause()
    {
        if (Status != AuctionSessionStatus.Active)
            throw new InvalidOperationException("Only active auctions can be paused");
        Status = AuctionSessionStatus.Paused;
    }

    public void Resume()
    {
        if (Status != AuctionSessionStatus.Paused)
            throw new InvalidOperationException("Only paused auctions can be resumed");
        Status = AuctionSessionStatus.Active;
    }

    public void Complete()
    {
        Status = AuctionSessionStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void SetCurrentPlayer(int playerId)
    {
        CurrentPlayerId = playerId;
    }

    public void NextRound()
    {
        RoundNumber++;
    }

    public void AddBid(Bid bid)
    {
        Bids.Add(bid);
    }
}
