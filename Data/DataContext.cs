using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Learntendo_backend.Models;

namespace Learntendo_backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public  DbSet<Exam> Exam { get; set; }

        public  DbSet<Subject> Subject { get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<User> User { get; set; }

        public DbSet<Admin> Admin { get; set; }

        public DbSet<Group> Group { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        public DbSet<ChatbotMessage> ChatbotMessages { get; set; }

        public DbSet<Message> Messages { get; set; }

       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>()
            .Property(g => g.StartDate)
            .HasColumnType("datetime2"); 

            modelBuilder.Entity<Group>()
                .Property(g => g.EndDate)
                .HasColumnType("datetime2");

            modelBuilder.Entity<Subject>()
               .HasOne(sc => sc.User)
               .WithMany(c => c.Subjects)
               .HasForeignKey(sc => sc.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Exam>()
               .HasOne(sc => sc.User)
               .WithMany(c => c.Exams)
               .HasForeignKey(sc => sc.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Exam>()
               .HasOne(sc => sc.Subject)
               .WithMany(c => c.Exams)
               .HasForeignKey(sc => sc.SubjectId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Message>()
               .HasOne(m => m.ChatbotMessage)        
               .WithMany(c => c.Messages)            
               .HasForeignKey(m => m.ChatId)         
               .OnDelete(DeleteBehavior.Cascade);   


            modelBuilder.Entity<ChatbotMessage>()
            .HasKey(c => c.ChatId);

            modelBuilder.Entity<Message>()
                .HasKey(m => m.MessageId);




            modelBuilder.Entity<Files>()
                .HasOne(f => f.User)
                .WithMany(u => u.Files)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Sender)
                .WithMany(u => u.SentRequests)
                .HasForeignKey(fr => fr.SenderId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Receiver)
                .WithMany(u => u.ReceivedRequests)
                .HasForeignKey(fr => fr.ReceiverId)
                .OnDelete(DeleteBehavior.Cascade); 




        }
    }
}
