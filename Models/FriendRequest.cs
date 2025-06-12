namespace Learntendo_backend.Models
{
    public class FriendRequest
    {
        public int Id { get; set; }

        public int? SenderId { get; set; }
        public User Sender { get; set; }

        public int? ReceiverId { get; set; }
        public User Receiver { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public FriendRequestStatus Status { get; set; }
    }

    public enum FriendRequestStatus
    {
        Pending,
        Accepted,
        Rejected
    }

}
