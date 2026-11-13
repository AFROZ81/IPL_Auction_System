namespace IPLAuctionSystem.Application.Interfaces;

using IPLAuctionSystem.Application.DTOs;

public interface ITeamService
{
    Task<IEnumerable<TeamDto>> GetAllTeamsAsync();
    Task<TeamDto?> GetTeamByIdAsync(int id);
    Task<TeamDto> CreateTeamAsync(CreateTeamDto dto);
    Task<TeamDto> UpdateTeamAsync(int id, UpdateTeamDto dto);
    Task<bool> DeleteTeamAsync(int id);
    Task<IEnumerable<PlayerDto>> GetTeamPlayersAsync(int teamId);
}

public interface IPlayerService
{
    Task<IEnumerable<PlayerDto>> GetAllPlayersAsync();
    Task<PlayerDto?> GetPlayerByIdAsync(int id);
    Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto dto);
    Task<PlayerDto> UpdatePlayerAsync(int id, UpdatePlayerDto dto);
    Task<bool> DeletePlayerAsync(int id);
    Task<bool> SellPlayerAsync(int playerId, int teamId, decimal bidAmount);
    Task<bool> MarkPlayerUnsoldAsync(int playerId);
}

public interface IAuctionService
{
    Task<AuctionSessionDto?> GetCurrentAuctionAsync();
    Task<AuctionSessionDto> StartAuctionAsync(string name, int roundNumber = 1);
    Task<bool> PauseAuctionAsync(int auctionSessionId);
    Task<bool> ResumeAuctionAsync(int auctionSessionId);
    Task<bool> CompleteAuctionAsync(int auctionSessionId);
    Task<bool> SetCurrentPlayerAsync(int auctionSessionId, int playerId);
    Task<bool> NextRoundAsync(int auctionSessionId);
}

public interface IBidService
{
    Task<BidDto> PlaceBidAsync(PlaceBidDto dto);
    Task<IEnumerable<BidDto>> GetBidsForPlayerAsync(int playerId);
    Task<IEnumerable<BidDto>> GetBidsForSessionAsync(int auctionSessionId);
    Task<bool> ValidateBidAsync(int playerId, int teamId, decimal amount);
}

public interface IFileStorage
{
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder);
    Task<bool> DeleteFileAsync(string filePath);
    Task<bool> FileExistsAsync(string filePath);
}
