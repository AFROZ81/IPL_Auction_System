namespace IPLAuctionSystem.Domain.Common;

public abstract class Entity
{
    public int Id { get; protected set; }

    protected Entity() { }

    protected Entity(int id)
    {
        Id = id;
    }
}

public abstract class DomainEvent
{
    public DateTime OccurredOn { get; protected set; } = DateTime.UtcNow;
}
