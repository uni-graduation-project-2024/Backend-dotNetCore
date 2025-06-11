using Learntendo_backend.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Learntendo_backend.Services
{
    public class LeaderboardService
    {
        private readonly IHubContext<LeaderboardHub> _hubContext;

        public LeaderboardService(IHubContext<LeaderboardHub> hubContext)
        {


            _hubContext = hubContext;
        }

        public async Task NotifyLeaderboardUpdate()
        {
            await _hubContext.Clients.All.SendAsync("ReceiveLeaderboardUpdate");
        }
    }
}