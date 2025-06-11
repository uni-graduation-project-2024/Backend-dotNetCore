using Learntendo_backend.Data;
using Learntendo_backend.Dtos;
using Learntendo_backend.Hubs;
using Learntendo_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace Learntendo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hub;
        private readonly DataContext _db; 

        public ChatController(IHubContext<ChatHub> hub, DataContext db)
        {
            _hub = hub;
            _db = db;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageDto dto)
        {
           
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId == null)
                return Unauthorized();

           
            if (string.IsNullOrEmpty(dto.ReceiverId) || string.IsNullOrEmpty(dto.Content))
                return BadRequest("ReceiverId and Content are required.");

           
            var message = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Content = dto.Content,
                SentAt = DateTime.UtcNow
            };

         
            _db.ChatMessages.Add(message);
            await _db.SaveChangesAsync();

           
            await _hub.Clients.User(senderId).SendAsync("ReceiveMessage", message);
            await _hub.Clients.User(dto.ReceiverId).SendAsync("ReceiveMessage", message);

            return Ok(new { Success = true, Message = message });
        }


        [HttpGet("history/{senderId}/{receiverId}")]
        public async Task<IActionResult> GetChatHistory(string senderId, string receiverId)
        {
            if (string.IsNullOrEmpty(senderId) || string.IsNullOrEmpty(receiverId))
                return BadRequest("Both senderId and receiverId are required");

            var messages = await _db.ChatMessages
                .Where(m =>
                    (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                    (m.SenderId == receiverId && m.ReceiverId == senderId))
                .OrderBy(m => m.SentAt)
                .Select(m => new ChatMessageDto
                {
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    Content = m.Content,
                    SentAt = m.SentAt
                })
                .ToListAsync();

            return Ok(messages);
        }

    }
}


