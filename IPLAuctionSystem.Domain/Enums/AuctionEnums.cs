namespace IPLAuctionSystem.Domain.Enums;

public enum AuctionStatus
{
    Pending = 0,
    Sold = 1,
    Unsold = 2
}

public enum PlayerCategory
{
    Batsman,
    Bowler,
    AllRounder,
    Wicketkeeper
}

public enum AuctionSessionStatus
{
    Scheduled = 0,
    Active = 1,
    Paused = 2,
    Completed = 3
}
