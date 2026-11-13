namespace IPLAuctionSystem.Infrastructure.Services.Implementations;

using IPLAuctionSystem.Application.DTOs;
using IPLAuctionSystem.Application.Interfaces;
using IPLAuctionSystem.Domain.Entities;
using IPLAuctionSystem.Domain.ValueObjects;
using IPLAuctionSystem.Infrastructure.Repositories;

public class BidService : IBidService
{
    private readonly IUnitOfWork _uow;

    public BidService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<BidDto> PlaceBidAsync(PlaceBidDto dto)
    {
        var bid = new Bid(0, dto.PlayerId, dto.TeamId, new Money(dto.Amount, "INR"));
        await _uow.Bids.AddAsync(bid);
        await _uow.SaveChangesAsync();

        return new BidDto
        {
            Id = bid.Id,
            AuctionSessionId = bid.AuctionSessionId,
            PlayerId = bid.PlayerId,
            TeamId = bid.TeamId,
            Amount = bid.Amount.Amount,
            BidTime = bid.BidTime,
            IsWinningBid = bid.IsWinningBid
        };
    }

    public async Task<IEnumerable<BidDto>> GetBidsForPlayerAsync(int playerId)
    {
        var bids = await _uow.Bids.GetBidsByPlayerAsync(playerId);
        return bids.Select(MapToDto);
    }

    public async Task<IEnumerable<BidDto>> GetBidsForSessionAsync(int auctionSessionId)
    {
        var bids = await _uow.Bids.GetBidsBySessionAsync(auctionSessionId);
        return bids.Select(MapToDto);
    }

    public async Task<bool> ValidateBidAsync(int playerId, int teamId, decimal amount)
    {
        var player = await _uow.Players.GetByIdAsync(playerId);
        var team = await _uow.Teams.GetByIdAsync(teamId);

        if (player == null || team == null) return false;
        if (amount <= 0) return false;
        if (team.Budget.Amount < amount) return false;

        return true;
    }

    private static BidDto MapToDto(Bid b) => new()
    {
        Id = b.Id,
        AuctionSessionId = b.AuctionSessionId,
        PlayerId = b.PlayerId,
        TeamId = b.TeamId,
        Amount = b.Amount.Amount,
        BidTime = b.BidTime,
        IsWinningBid = b.IsWinningBid
    };
}
