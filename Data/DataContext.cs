using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Learntendo_backend.Models;

namespace Learntendo_backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public  DbSet<Exam> Exam { get; set; }

        public  DbSet<Subject> Subject { get; set; }
        public DbSet<Files> Files { get; set; }
        public DbSet<User> User { get; set; }

        public DbSet<Admin> Admin { get; set; }

        public DbSet<Group> Group { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>()
            .Property(g => g.StartDate)
            .HasColumnType("datetime2"); // تأكد من دعم النوع في قاعدة البيانات

            modelBuilder.Entity<Group>()
                .Property(g => g.EndDate)
                .HasColumnType("datetime2");

            modelBuilder.Entity<Subject>()
               .HasOne(sc => sc.User)
               .WithMany(c => c.Subjects)
               .HasForeignKey(sc => sc.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Exam>()
               .HasOne(sc => sc.User)
               .WithMany(c => c.Exams)
               .HasForeignKey(sc => sc.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Exam>()
               .HasOne(sc => sc.Subject)
               .WithMany(c => c.Exams)
               .HasForeignKey(sc => sc.SubjectId)
               .OnDelete(DeleteBehavior.Restrict);

         
        }
    }
}
