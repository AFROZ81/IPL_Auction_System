namespace IPLAuctionSystem.Infrastructure.Repositories;

using IPLAuctionSystem.Domain.Entities;
using IPLAuctionSystem.Domain.ValueObjects;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<bool> ExistsAsync(int id);
    Task<int> SaveChangesAsync();
}

public interface ITeamRepository : IRepository<Team>
{
    Task<Team?> GetTeamWithPlayersAsync(int id);
    Task<IEnumerable<Team>> GetTeamsWithPlayersAsync();
}

public interface IPlayerRepository : IRepository<Player>
{
    Task<Player?> GetPlayerWithTeamAsync(int id);
    Task<IEnumerable<Player>> GetPlayersByStatusAsync(string status);
    Task<IEnumerable<Player>> GetPlayersByCategoryAsync(string category);
}

public interface IAuctionSessionRepository : IRepository<AuctionSession>
{
    Task<AuctionSession?> GetActiveSessionAsync();
    Task<AuctionSession?> GetSessionWithBidsAsync(int id);
}

public interface IBidRepository : IRepository<Bid>
{
    Task<IEnumerable<Bid>> GetBidsByPlayerAsync(int playerId);
    Task<IEnumerable<Bid>> GetBidsBySessionAsync(int sessionId);
    Task<Bid?> GetWinningBidAsync(int sessionId, int playerId);
}

public interface IUnitOfWork
{
    ITeamRepository Teams { get; }
    IPlayerRepository Players { get; }
    IAuctionSessionRepository AuctionSessions { get; }
    IBidRepository Bids { get; }
    Task<int> SaveChangesAsync();
}
