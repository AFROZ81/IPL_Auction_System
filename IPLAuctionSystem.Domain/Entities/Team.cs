namespace IPLAuctionSystem.Domain.Entities;

using IPLAuctionSystem.Domain.Common;
using IPLAuctionSystem.Domain.Enums;
using IPLAuctionSystem.Domain.ValueObjects;

public class Team : Entity
{
    public string Name { get; set; } = string.Empty;
    public string? Owner { get; set; }
    public Money Budget { get; set; } = new(1000000000m, "INR");
    public string? TeamLogo { get; set; }
    public List<Player> Players { get; set; } = new();

    private Team() { } // EF Core

    public Team(string name, string? owner, Money budget, string? teamLogo = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Owner = owner;
        Budget = budget;
        TeamLogo = teamLogo;
    }

    public void UpdateDetails(string name, string? owner, Money budget, string? teamLogo = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Owner = owner;
        Budget = budget;
        TeamLogo = teamLogo;
    }

    public void SetLogo(string? logoPath)
    {
        TeamLogo = logoPath;
    }

    public TeamBudget DeductBudget(Money amount)
    {
        if (!Budget.CanAfford(amount))
            throw new InvalidOperationException("Insufficient budget");

        Budget = new Money(Budget.Amount - amount.Amount, Budget.Currency);
        return new TeamBudget(
            new Money(Budget.Amount + amount.Amount, Budget.Currency),
            Budget,
            new Money(amount.Amount, Budget.Currency)
        );
    }

    public void AddPlayer(Player player)
    {
        if (Players.Count >= 25)
            throw new InvalidOperationException("Squad is full (max 25 players)");
        Players.Add(player);
    }

    public void RemovePlayer(Player player)
    {
        Players.Remove(player);
    }
}
