
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Learntendo_backend.Hubs
{
    /*[Authorize]*/  
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            
            await base.OnConnectedAsync();
        }

       
        public async Task SendMessage(string receiverId, string message)
        {
            var senderId = Context.UserIdentifier;
            if (string.IsNullOrEmpty(senderId)) return;

           
            await Clients.User(senderId).SendAsync("ReceiveMessage", new
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = message,
                SentAt = System.DateTime.UtcNow
            });

            await Clients.User(receiverId).SendAsync("ReceiveMessage", new
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = message,
                SentAt = System.DateTime.UtcNow
            });
        }
    }
}



