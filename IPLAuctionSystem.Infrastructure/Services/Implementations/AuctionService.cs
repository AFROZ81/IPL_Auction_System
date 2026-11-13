namespace IPLAuctionSystem.Infrastructure.Services.Implementations;

using IPLAuctionSystem.Application.DTOs;
using IPLAuctionSystem.Application.Interfaces;
using IPLAuctionSystem.Domain.Entities;
using IPLAuctionSystem.Domain.Enums;
using IPLAuctionSystem.Domain.ValueObjects;
using IPLAuctionSystem.Infrastructure.Repositories;

public class AuctionService : IAuctionService
{
    private readonly IUnitOfWork _uow;

    public AuctionService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<AuctionSessionDto?> GetCurrentAuctionAsync()
    {
        var session = await _uow.AuctionSessions.GetActiveSessionAsync();
        return session == null ? null : MapToDto(session);
    }

    public async Task<AuctionSessionDto> StartAuctionAsync(string name, int roundNumber = 1)
    {
        var session = new AuctionSession(name, roundNumber);
        session.Start();
        await _uow.AuctionSessions.AddAsync(session);
        await _uow.SaveChangesAsync();
        return MapToDto(session);
    }

    public async Task<bool> PauseAuctionAsync(int auctionSessionId)
    {
        var session = await _uow.AuctionSessions.GetByIdAsync(auctionSessionId);
        if (session == null) return false;
        session.Pause();
        _uow.AuctionSessions.Update(session);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ResumeAuctionAsync(int auctionSessionId)
    {
        var session = await _uow.AuctionSessions.GetByIdAsync(auctionSessionId);
        if (session == null) return false;
        session.Resume();
        _uow.AuctionSessions.Update(session);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CompleteAuctionAsync(int auctionSessionId)
    {
        var session = await _uow.AuctionSessions.GetByIdAsync(auctionSessionId);
        if (session == null) return false;
        session.Complete();
        _uow.AuctionSessions.Update(session);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetCurrentPlayerAsync(int auctionSessionId, int playerId)
    {
        var session = await _uow.AuctionSessions.GetByIdAsync(auctionSessionId);
        if (session == null) return false;
        session.SetCurrentPlayer(playerId);
        _uow.AuctionSessions.Update(session);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<bool> NextRoundAsync(int auctionSessionId)
    {
        var session = await _uow.AuctionSessions.GetByIdAsync(auctionSessionId);
        if (session == null) return false;
        session.NextRound();
        _uow.AuctionSessions.Update(session);
        await _uow.SaveChangesAsync();
        return true;
    }

    private static AuctionSessionDto MapToDto(AuctionSession s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        Status = s.Status.ToString(),
        CurrentPlayerId = s.CurrentPlayerId,
        CurrentPlayerName = s.CurrentPlayer?.Name,
        RoundNumber = s.RoundNumber,
        StartedAt = s.StartedAt,
        CompletedAt = s.CompletedAt,
        RecentBids = s.Bids.Select(b => new BidDto
        {
            Id = b.Id,
            AuctionSessionId = b.AuctionSessionId,
            PlayerId = b.PlayerId,
            TeamId = b.TeamId,
            Amount = b.Amount.Amount,
            BidTime = b.BidTime,
            IsWinningBid = b.IsWinningBid
        }).ToList()
    };
}
