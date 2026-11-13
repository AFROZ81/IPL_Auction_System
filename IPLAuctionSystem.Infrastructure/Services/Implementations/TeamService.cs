namespace IPLAuctionSystem.Infrastructure.Services.Implementations;

using IPLAuctionSystem.Application.DTOs;
using IPLAuctionSystem.Application.Interfaces;
using IPLAuctionSystem.Domain.Entities;
using IPLAuctionSystem.Domain.Enums;
using IPLAuctionSystem.Domain.ValueObjects;
using IPLAuctionSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

public class TeamService : ITeamService
{
    private readonly IUnitOfWork _uow;

    public TeamService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<IEnumerable<TeamDto>> GetAllTeamsAsync()
    {
        var teams = await _uow.Teams.GetTeamsWithPlayersAsync();
        return teams.Select(MapToDto);
    }

    public async Task<TeamDto?> GetTeamByIdAsync(int id)
    {
        var team = await _uow.Teams.GetTeamWithPlayersAsync(id);
        return team == null ? null : MapToDto(team);
    }

    public async Task<TeamDto> CreateTeamAsync(CreateTeamDto dto)
    {
        var team = new Team(dto.Name, dto.Owner, new Money(dto.Budget, "INR"));
        await _uow.Teams.AddAsync(team);
        await _uow.SaveChangesAsync();
        return MapToDto(team);
    }

    public async Task<TeamDto> UpdateTeamAsync(int id, UpdateTeamDto dto)
    {
        var team = await _uow.Teams.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Team {id} not found");
        team.UpdateDetails(dto.Name, dto.Owner, new Money(dto.Budget, "INR"));
        _uow.Teams.Update(team);
        await _uow.SaveChangesAsync();
        return MapToDto(team);
    }

    public async Task<bool> DeleteTeamAsync(int id)
    {
        var team = await _uow.Teams.GetByIdAsync(id);
        if (team == null) return false;
        _uow.Teams.Remove(team);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<PlayerDto>> GetTeamPlayersAsync(int teamId)
    {
        var team = await _uow.Teams.GetTeamWithPlayersAsync(teamId);
        if (team == null) return Enumerable.Empty<PlayerDto>();
        return team.Players.Select(MapPlayerToDto);
    }

    private static TeamDto MapToDto(Team team) => new()
    {
        Id = team.Id,
        Name = team.Name,
        Owner = team.Owner,
        Budget = team.Budget.Amount,
        TeamLogo = team.TeamLogo,
        PlayerCount = team.Players.Count
    };

    private static PlayerDto MapPlayerToDto(Player p) => new()
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
