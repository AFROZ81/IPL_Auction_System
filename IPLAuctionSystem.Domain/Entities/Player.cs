namespace IPLAuctionSystem.Domain.Entities;

using IPLAuctionSystem.Domain.Common;
using IPLAuctionSystem.Domain.Enums;
using IPLAuctionSystem.Domain.ValueObjects;

public class Player : Entity
{
    public string Name { get; set; } = string.Empty;
    public PlayerCategory Category { get; set; }
    public Money BasePrice { get; set; } = new(0m, "INR");
    public Money SoldPrice { get; set; } = new(0m, "INR");
    public AuctionStatus AuctionStatus { get; set; } = AuctionStatus.Pending;
    public string? ProfilePicture { get; set; }
    public int? TeamId { get; set; }
    public Team? Team { get; set; }

    private Player() { } // EF Core

    public Player(string name, PlayerCategory category, Money basePrice, string? profilePicture = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Category = category;
        BasePrice = basePrice;
        ProfilePicture = profilePicture;
    }

    public void UpdateDetails(string name, PlayerCategory category, Money basePrice, string? profilePicture = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Category = category;
        BasePrice = basePrice;
        ProfilePicture = profilePicture;
    }

    public void MarkAsSold(Team team, Money soldPrice)
    {
        if (AuctionStatus != AuctionStatus.Pending && AuctionStatus != AuctionStatus.Unsold)
            throw new InvalidOperationException("Player is not available for sale");

        AuctionStatus = AuctionStatus.Sold;
        SoldPrice = soldPrice;
        TeamId = team.Id;
        Team = team;
        team.AddPlayer(this);
    }

    public void MarkAsUnsold()
    {
        AuctionStatus = AuctionStatus.Unsold;
        SoldPrice = new Money(0m, "INR");
        TeamId = null;
        Team = null;
    }

    public void ResetToPending()
    {
        AuctionStatus = AuctionStatus.Pending;
        SoldPrice = new Money(0m, "INR");
        TeamId = null;
        Team = null;
    }
}
