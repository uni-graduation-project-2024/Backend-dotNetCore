﻿// <auto-generated />
using System;
using Learntendo_backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Learntendo_backend.Migrations
{
    [DbContext(typeof(DataContext))]
//<<<<<<<< HEAD:Migrations/20250129162137_addtable.Designer.cs
    [Migration("20250129162137_addtable")]
   // partial class addtable
//========
   // [Migration("20250129145913_llll")]
    partial class llll
//>>>>>>>> 4a92f936a1676dffa49d422b2d8801254d4f37e8:Migrations/20250129145913_llll.Designer.cs
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Learntendo_backend.Models.Admin", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Admin");
                });

            modelBuilder.Entity("Learntendo_backend.Models.Exam", b =>
                {
                    b.Property<int>("ExamId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ExamId"));

                    b.Property<string>("DifficultyLevel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("McqQuestionsData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("NumCorrectQuestions")
                        .HasColumnType("int");

                    b.Property<int?>("NumQuestions")
                        .HasColumnType("int");

                    b.Property<int?>("NumWrongQuestions")
                        .HasColumnType("int");

                    b.Property<string>("QuestionType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SubjectId")
                        .HasColumnType("int");

                    b.Property<string>("TfQuestionsData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeOnly?>("TimeTaken")
                        .HasColumnType("time");

                    b.Property<int?>("TotalScore")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.Property<int?>("XpCollected")
                        .HasColumnType("int");

                    b.HasKey("ExamId");

                    b.HasIndex("SubjectId");

                    b.HasIndex("UserId");

                    b.ToTable("Exam");
                });

            modelBuilder.Entity("Learntendo_backend.Models.Subject", b =>
                {
                    b.Property<int>("SubjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SubjectId"));

                    b.Property<int>("NumExams")
                        .HasColumnType("int");

                    b.Property<string>("SubjectName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TotalQuestions")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("SubjectId");

                    b.HasIndex("UserId");

                    b.ToTable("Subject");
                });

            modelBuilder.Entity("Learntendo_backend.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<int>("Coins")
                        .HasColumnType("int");

                    b.Property<bool>("CompleteDailyChallenge")
                        .HasColumnType("bit");

                    b.Property<bool>("CompleteMonthlyChallenge")
                        .HasColumnType("bit");

                    b.Property<bool>("CompleteWeeklyChallenge")
                        .HasColumnType("bit");

                    b.Property<string>("CurrentLeague")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DailyXp")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DateCompleteDailyChallenge")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateCompleteMonthlyChallenge")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateCompleteWeeklyChallenge")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FreezeStreak")
                        .HasColumnType("int");

                    b.Property<bool?>("IfStreakActive")
                        .HasColumnType("bit");

                    b.Property<DateTime>("JoinedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("MonthlyXp")
                        .HasColumnType("int");

                    b.Property<int>("NumQuestionSolToday")
                        .HasColumnType("int");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("PasswordSalt")
                        .HasColumnType("varbinary(max)");

                    b.Property<int>("StreakScore")
                        .HasColumnType("int");

                    b.Property<int>("TotalQuestion")
                        .HasColumnType("int");

                    b.Property<int>("TotalXp")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WeeklyXp")
                        .HasColumnType("int");

                    b.HasKey("UserId");

                    b.ToTable("User");
                });

            modelBuilder.Entity("Learntendo_backend.Models.Exam", b =>
                {
                    b.HasOne("Learntendo_backend.Models.Subject", "Subject")
                        .WithMany("Exams")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Learntendo_backend.Models.User", "User")
                        .WithMany("Exams")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Subject");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Learntendo_backend.Models.Subject", b =>
                {
                    b.HasOne("Learntendo_backend.Models.User", "User")
                        .WithMany("Subjects")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Learntendo_backend.Models.Subject", b =>
                {
                    b.Navigation("Exams");
                });

            modelBuilder.Entity("Learntendo_backend.Models.User", b =>
                {
                    b.Navigation("Exams");

                    b.Navigation("Subjects");
                });
#pragma warning restore 612, 618
        }
    }
}
