using AutoMapper;
using Learntendo_backend.Data;
using Learntendo_backend.Dtos;
using Learntendo_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Learntendo_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FriendsController : Controller
    {
        private readonly DataContext _db;
        private readonly IDataRepository<User> _userRepo;
        private readonly IMapper _map;

        public FriendsController(DataContext db, IDataRepository<User> userRepo, IMapper map)
        {
            _db = db;
            _userRepo = userRepo;
            _map = map;
        }

        [HttpGet("suggestion-friends/{userId}")]
        public async Task<IActionResult> GetSuggestionFriends(int userId)
        {
            var sentRequests = _db.FriendRequests
                .Where(r => r.SenderId == userId)
                .Select(r => r.ReceiverId);

            var receivedRequests = _db.FriendRequests
                .Where(r => r.ReceiverId == userId)
                .Select(r => r.SenderId);

            var relations = sentRequests.Concat(receivedRequests);

            var suggestionFriends = _db.User
                .Where(u => u.UserId != userId && !relations.Contains(u.UserId))
                .ToList();

            var result = new List<FriendsDto>();

            foreach (var user in suggestionFriends)
            {
                result.Add(new FriendsDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    ProfilePicture = await _userRepo.GetBase64ImageAsync(user.ProfilePicturePath)
                });
            }

            return Ok(result);
        }

        [HttpGet("friends/{userId}")]
        public async Task<IActionResult> GetFriends(int userId)
        {
            var friends = _db.FriendRequests
                .Where(r =>
                    (r.SenderId == userId || r.ReceiverId == userId) &&
                    r.Status == FriendRequestStatus.Accepted)
                .Select(r => r.SenderId == userId ? r.Receiver : r.Sender)
                .ToList();

            var result = new List<FriendsDto>();

            foreach (var user in friends)
            {
                result.Add(new FriendsDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    ProfilePicture = await _userRepo.GetBase64ImageAsync(user.ProfilePicturePath)
                });
            }

            return Ok(result);
        }

        [HttpPost("send-request")]
        public async Task<IActionResult> SendFriendRequest(int senderId, int receiverId)
        {
            if (_db.FriendRequests.Any(r =>
                (r.SenderId == senderId && r.ReceiverId == receiverId) ||
                (r.SenderId == receiverId && r.ReceiverId == senderId)))
            {
                return BadRequest("Friend request already exists");
            }

            var request = new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Status = FriendRequestStatus.Pending
            };

            await _db.FriendRequests.AddAsync(request);
            await _db.SaveChangesAsync();

            return Ok("Friend request sent.");
        }

        [HttpPost("respond-request")]
        public async Task<IActionResult> RespondToFriendRequest(int requestId, bool accept)
        {
            var request = _db.FriendRequests.Find(requestId);
            if (request == null)
                return NotFound("Request not found.");

            request.Status = accept ? FriendRequestStatus.Accepted : FriendRequestStatus.Rejected;

            await _db.SaveChangesAsync();
            return Ok($"Request has been {(accept ? "accepted" : "rejected")}");
        }


        [HttpGet("received-requests/{userId}")]
        public async Task<IActionResult> GetReceivedFriendRequests(int userId)
        {
            var receivedRequests = await _db.FriendRequests
                .Where(r => r.ReceiverId == userId && r.Status == FriendRequestStatus.Pending)

                
                .Include(r => r.Sender) 

                .ToListAsync();

            var result = new List<FriendsDto>();

            foreach (var request in receivedRequests)
            {
                result.Add(new FriendsDto

                {
                    RequestId = request.Id,
                    UserId = request.Sender.UserId,
                    Username = request.Sender.Username,
                    ProfilePicture = await _userRepo.GetBase64ImageAsync(request.Sender.ProfilePicturePath)
                });
            }

            return Ok(result);
        }

        [HttpDelete("remove-friend")]
        public async Task<IActionResult> RemoveFriend(int userId, int friendId)
        {
            var friendRequest = await _db.FriendRequests
                .FirstOrDefaultAsync(r =>
                    ((r.SenderId == userId && r.ReceiverId == friendId) ||
                    (r.SenderId == friendId && r.ReceiverId == userId)) &&
                    r.Status == FriendRequestStatus.Accepted);

            if (friendRequest == null)
            {
                return NotFound("Friend not found.");
            }

           
            var userIdStr = userId.ToString();
            var friendIdStr = friendId.ToString();

            var messages = await _db.ChatMessages
                .Where(m =>
                    (m.SenderId == userIdStr && m.ReceiverId == friendIdStr) ||
                    (m.SenderId == friendIdStr && m.ReceiverId == userIdStr))
                .ToListAsync();


            if (messages.Any())
            {
                _db.ChatMessages.RemoveRange(messages);
            }

            
            _db.FriendRequests.Remove(friendRequest);
            await _db.SaveChangesAsync();

            return Ok("Friend and related messages removed successfully.");
        }


    }

}

