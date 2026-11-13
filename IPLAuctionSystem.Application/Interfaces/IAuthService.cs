namespace IPLAuctionSystem.Application.Interfaces;

using IPLAuctionSystem.Application.DTOs;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
    Task<bool> LogoutAsync();
    Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
}
