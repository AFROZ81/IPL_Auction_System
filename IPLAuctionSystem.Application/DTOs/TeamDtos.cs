namespace IPLAuctionSystem.Application.DTOs;

public class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Owner { get; set; }
    public decimal Budget { get; set; }
    public string? TeamLogo { get; set; }
    public int PlayerCount { get; set; }
    public int RemainingSlots => 25 - PlayerCount;
}

public class CreateTeamDto
{
    public string Name { get; set; } = string.Empty;
    public string? Owner { get; set; }
    public decimal Budget { get; set; }
}

public class UpdateTeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Owner { get; set; }
    public decimal Budget { get; set; }
}
