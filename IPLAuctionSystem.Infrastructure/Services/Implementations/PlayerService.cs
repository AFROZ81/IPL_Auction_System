namespace IPLAuctionSystem.Infrastructure.Services.Implementations;

using IPLAuctionSystem.Application.DTOs;
using IPLAuctionSystem.Application.Interfaces;
using IPLAuctionSystem.Domain.Entities;
using IPLAuctionSystem.Domain.Enums;
using IPLAuctionSystem.Domain.ValueObjects;
using IPLAuctionSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

public class PlayerService : IPlayerService
{
    private readonly IUnitOfWork _uow;

    public PlayerService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<PlayerDto>> GetAllPlayersAsync()
    {
        var players = await _uow.Players.GetAllAsync();
        return players.Select(MapToDto);
    }

    public async Task<PlayerDto?> GetPlayerByIdAsync(int id)
    {
        var player = await _uow.Players.GetPlayerWithTeamAsync(id);
        return player == null ? null : MapToDto(player);
    }

    public async Task<PlayerDto> CreatePlayerAsync(CreatePlayerDto dto)
    {
        if (!Enum.TryParse<PlayerCategory>(dto.Category, true, out var category))
            throw new ArgumentException($"Invalid category: {dto.Category}");

        var player = new Player(dto.Name, category, new Money(dto.BasePrice, "INR"));
        await _uow.Players.AddAsync(player);
        await _uow.SaveChangesAsync();
        return MapToDto(player);
    }

    public async Task<PlayerDto> UpdatePlayerAsync(int id, UpdatePlayerDto dto)
    {
        var player = await _uow.Players.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Player {id} not found");

        if (Enum.TryParse<PlayerCategory>(dto.Category, true, out var category))
            player.UpdateDetails(dto.Name, category, new Money(dto.BasePrice, "INR"));

        if (Enum.TryParse<AuctionStatus>(dto.AuctionStatus, true, out var status))
        {
            player.GetType().GetProperty("AuctionStatus")?.SetValue(player, status);
        }

        _uow.Players.Update(player);
        await _uow.SaveChangesAsync();
        return MapToDto(player);
    }

    public async Task<bool> DeletePlayerAsync(int id)
    {
        var player = await _uow.Players.GetByIdAsync(id);
        if (player == null) return false;
        _uow.Players.Remove(player);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SellPlayerAsync(int playerId, int teamId, decimal bidAmount)
    {
        var player = await _uow.Players.GetPlayerWithTeamAsync(playerId);
        var team = await _uow.Teams.GetByIdAsync(teamId);

        if (player == null || team == null) return false;
        if (team.Budget.Amount < bidAmount) return false;

        var money = new Money(bidAmount, "INR");
        player.MarkAsSold(team, money);

        _uow.Players.Update(player);
        _uow.Teams.Update(team);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkPlayerUnsoldAsync(int playerId)
    {
        var player = await _uow.Players.GetByIdAsync(playerId);
        if (player == null) return false;

        player.MarkAsUnsold();
        _uow.Players.Update(player);
        await _uow.SaveChangesAsync();
        return true;
    }

    private static PlayerDto MapToDto(Player p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Category = p.Category.ToString(),
        BasePrice = p.BasePrice.Amount,
        SoldPrice = p.SoldPrice.Amount,
        AuctionStatus = p.AuctionStatus.ToString(),
        ProfilePicture = p.ProfilePicture,
        TeamId = p.TeamId,
        TeamName = p.Team?.Name
    };
}
