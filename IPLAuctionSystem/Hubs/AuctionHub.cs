using Microsoft.AspNetCore.SignalR;

namespace IPLAuctionSystem.Hubs;

public class AuctionHub : Hub
{
    public async Task SendBid(string teamName, string amount)
    {
        // This sends the bid to everyone viewing the page
        await Clients.All.SendAsync("ReceiveNewBid", teamName, amount);
    }
}