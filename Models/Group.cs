namespace Learntendo_backend.Models
{
    public class Group
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }
        public ICollection<User> Users { get; set; }
    }

}
