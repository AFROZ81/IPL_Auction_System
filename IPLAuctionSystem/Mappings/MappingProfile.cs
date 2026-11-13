namespace IPLAuctionSystem.Mappings;

using AutoMapper;
using IPLAuctionSystem.Application.DTOs;
using IPLAuctionSystem.Domain.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Team, TeamDto>()
            .ForMember(d => d.PlayerCount, o => o.MapFrom(s => s.Players.Count));

        CreateMap<CreateTeamDto, Team>();
        CreateMap<UpdateTeamDto, Team>();

        CreateMap<Player, PlayerDto>()
            .ForMember(d => d.TeamName, o => o.MapFrom(s => s.Team!.Name));

        CreateMap<CreatePlayerDto, Player>();
        CreateMap<UpdatePlayerDto, Player>();

        CreateMap<AuctionSession, AuctionSessionDto>();
        CreateMap<Bid, BidDto>();

        CreateMap<PlaceBidDto, Bid>();
    }
}
