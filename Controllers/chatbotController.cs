using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using Learntendo_backend.Models;
using Learntendo_backend.Data;
using Microsoft.EntityFrameworkCore;
using Learntendo_backend.Dtos;
namespace Learntendo_backend.Controllers
{


   
        [ApiController]
        [Route("api/[controller]")]
        public class ChatbotController : ControllerBase
        {
            private readonly DataContext _context;

            public ChatbotController(DataContext context)
            {
                _context = context;
            }

            
            [HttpGet("{chatId}/messages")]
            public async Task<IActionResult> GetChatMessagesByChatId(int chatId)
            {
                var messages = await _context.Messages
                    .Where(m => m.ChatId == chatId)
                    .OrderBy(m => m.ChatDateTime)
                    .ToListAsync();

                if (messages == null || messages.Count == 0)
                    return NotFound(new { message = "No messages found for this chat" });

                return Ok(messages);
            }

            [HttpPut("changename")]
            public async Task<IActionResult> ChangeChatName([FromBody] ChatbotDTO dto)
           {
                 var chat = await _context.ChatbotMessages.FindAsync(dto.ChatId);

                if (chat == null)
                   return NotFound(new { message = "Chat not found" });

               chat.ChatName = dto.ChatName;

              await _context.SaveChangesAsync();

              return Ok(new
              {
                message = "Chat name updated successfully",
                newName = dto.ChatName
              });
           }


        [HttpPost("createchat")]
            public async Task<IActionResult> CreateChat([FromBody] CreatechatDto dto)
            {
                var chat = new ChatbotMessage
                {
                    ChatName = dto.ChatName,
                    UserId = dto.UserId,
                    ChatDateTime = dto.ChatDateTime
                };

                _context.ChatbotMessages.Add(chat);
                await _context.SaveChangesAsync();

                return Ok(new { chatId = chat.ChatId });
            }

        
            [HttpPost("savemessage")]
            public async Task<IActionResult> SaveMessage([FromBody] SavemessageDto dto)
            {
                
                var chat = await _context.ChatbotMessages.FindAsync(dto.ChatId);
                if (chat == null)
                    return NotFound(new { message = "Chat not found" });

                var message = new Message
                {
                    ChatId = dto.ChatId,
                    UserId = dto.UserId,
                    UserMessage = dto.UserMessages,
                    AiResponse = dto.AiResponse,
                    ChatDateTime = dto.ChatDateTime,
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Message saved successfully", messageId = message.MessageId });
            }
        }

    }





