namespace IPLAuctionSystem.Domain.Events;

using IPLAuctionSystem.Domain.Common;
using IPLAuctionSystem.Domain.Entities;
using IPLAuctionSystem.Domain.ValueObjects;

public class PlayerSold : DomainEvent
{
    public Player Player { get; init; }
    public Team Team { get; init; }
    public Money SoldPrice { get; init; }

    public PlayerSold(Player player, Team team, Money soldPrice)
    {
        Player = player;
        Team = team;
        SoldPrice = soldPrice;
    }
}

public class BidPlaced : DomainEvent
{
    public Bid Bid { get; init; }
    public int AuctionSessionId { get; init; }

    public BidPlaced(Bid bid, int auctionSessionId)
    {
        Bid = bid;
        AuctionSessionId = auctionSessionId;
    }
}

public class AuctionStarted : DomainEvent
{
    public int AuctionSessionId { get; init; }
    public string AuctionSessionName { get; init; }

    public AuctionStarted(int auctionSessionId, string auctionSessionName)
    {
        AuctionSessionId = auctionSessionId;
        AuctionSessionName = auctionSessionName;
    }
}

public class AuctionEnded : DomainEvent
{
    public int AuctionSessionId { get; init; }

    public AuctionEnded(int auctionSessionId)
    {
        AuctionSessionId = auctionSessionId;
    }
}
