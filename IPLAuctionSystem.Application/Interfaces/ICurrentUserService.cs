namespace IPLAuctionSystem.Application.Interfaces;

using System.Security.Claims;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? UserName { get; }
    string? Email { get; }
    IEnumerable<string> Roles { get; }
    bool IsInRole(string role);
    bool IsAuthenticated { get; }
}
