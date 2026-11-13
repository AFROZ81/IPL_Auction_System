namespace IPLAuctionSystem.Infrastructure.Repositories;

using IPLAuctionSystem.Domain.Entities;
using IPLAuctionSystem.Domain.ValueObjects;
using IPLAuctionSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public virtual async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public virtual void Update(T entity) => _dbSet.Update(entity);

    public virtual void Remove(T entity) => _dbSet.Remove(entity);

    public virtual async Task<bool> ExistsAsync(int id) => await _dbSet.AnyAsync(e => EF.Property<int>(e, "Id") == id);

    public virtual async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
}

public class TeamRepository : Repository<Team>, ITeamRepository
{
    public TeamRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Team?> GetTeamWithPlayersAsync(int id) =>
        await _context.Teams.Include(t => t.Players).FirstOrDefaultAsync(t => t.Id == id);

    public async Task<IEnumerable<Team>> GetTeamsWithPlayersAsync() =>
        await _context.Teams.Include(t => t.Players).ToListAsync();
}

public class PlayerRepository : Repository<Player>, IPlayerRepository
{
    public PlayerRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Player?> GetPlayerWithTeamAsync(int id) =>
        await _context.Players.Include(p => p.Team).FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Player>> GetPlayersByStatusAsync(string status) =>
        await _context.Players.Where(p => p.AuctionStatus.ToString() == status).ToListAsync();

    public async Task<IEnumerable<Player>> GetPlayersByCategoryAsync(string category) =>
        await _context.Players.Where(p => p.Category.ToString() == category).ToListAsync();
}

public class AuctionSessionRepository : Repository<AuctionSession>, IAuctionSessionRepository
{
    public AuctionSessionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<AuctionSession?> GetActiveSessionAsync() =>
        await _context.AuctionSessions.FirstOrDefaultAsync(a => a.Status == Domain.Enums.AuctionSessionStatus.Active);

    public async Task<AuctionSession?> GetSessionWithBidsAsync(int id) =>
        await _context.AuctionSessions.Include(a => a.Bids).FirstOrDefaultAsync(a => a.Id == id);
}

public class BidRepository : Repository<Bid>, IBidRepository
{
    public BidRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Bid>> GetBidsByPlayerAsync(int playerId) =>
        await _context.Bids.Where(b => b.PlayerId == playerId).ToListAsync();

    public async Task<IEnumerable<Bid>> GetBidsBySessionAsync(int sessionId) =>
        await _context.Bids.Where(b => b.AuctionSessionId == sessionId).ToListAsync();

    public async Task<Bid?> GetWinningBidAsync(int sessionId, int playerId) =>
        await _context.Bids.FirstOrDefaultAsync(b => b.AuctionSessionId == sessionId && b.PlayerId == playerId && b.IsWinningBid);
}

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ApplicationDbContext _context;
    private ITeamRepository? _teams;
    private IPlayerRepository? _players;
    private IAuctionSessionRepository? _auctionSessions;
    private IBidRepository? _bids;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public ITeamRepository Teams => _teams ??= new TeamRepository(_context);
    public IPlayerRepository Players => _players ??= new PlayerRepository(_context);
    public IAuctionSessionRepository AuctionSessions => _auctionSessions ??= new AuctionSessionRepository(_context);
    public IBidRepository Bids => _bids ??= new BidRepository(_context);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
