namespace IPLAuctionSystem.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "INR";

    public Money() { } // EF Core

    public Money(decimal amount, string currency = "INR")
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money operator +(Money a, Money b) => new(a.Amount + b.Amount, a.Currency);
    public static Money operator -(Money a, Money b) => new(a.Amount - b.Amount, a.Currency);
    public static bool operator >(Money a, Money b) => a.Amount > b.Amount;
    public static bool operator <(Money a, Money b) => a.Amount < b.Amount;
    public static bool operator >=(Money a, Money b) => a.Amount >= b.Amount;
    public static bool operator <=(Money a, Money b) => a.Amount <= b.Amount;

    public bool CanAfford(Money amount) => this.Amount >= amount.Amount;

    public override string ToString() => $"{Currency} {Amount:N0}";
}

public record TeamBudget
{
    public Money TotalBudget { get; init; }
    public Money RemainingBudget { get; init; }
    public Money SpentBudget { get; init; }

    public TeamBudget(Money totalBudget, Money remainingBudget, Money spentBudget)
    {
        TotalBudget = totalBudget;
        RemainingBudget = remainingBudget;
        SpentBudget = spentBudget;
    }

    public bool CanAfford(Money amount) => RemainingBudget >= amount;

    public TeamBudget Deduct(Money amount) => new(
        TotalBudget,
        new Money(RemainingBudget.Amount - amount.Amount, RemainingBudget.Currency),
        new Money(SpentBudget.Amount + amount.Amount, SpentBudget.Currency)
    );
}
