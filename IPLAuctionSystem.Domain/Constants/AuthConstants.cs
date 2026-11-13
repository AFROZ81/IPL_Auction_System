namespace IPLAuctionSystem.Domain.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Auctioneer = "Auctioneer";
    public const string FranchiseOwner = "FranchiseOwner";
    public const string Viewer = "Viewer";
}

public static class Policies
{
    public const string CanEditOwnTeam = "CanEditOwnTeam";
    public const string CanRunAuction = "CanRunAuction";
    public const string CanViewFinancials = "CanViewFinancials";
}
