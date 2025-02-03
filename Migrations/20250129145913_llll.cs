using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learntendo_backend.Migrations
{
  
    public partial class llll : Migration

    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admin", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalQuestion = table.Column<int>(type: "int", nullable: false),
                    NumQuestionSolToday = table.Column<int>(type: "int", nullable: false),
                    Coins = table.Column<int>(type: "int", nullable: false),
                    StreakScore = table.Column<int>(type: "int", nullable: false),
                    FreezeStreak = table.Column<int>(type: "int", nullable: false),
                    TotalXp = table.Column<int>(type: "int", nullable: false),
                    DailyXp = table.Column<int>(type: "int", nullable: false),
                    WeeklyXp = table.Column<int>(type: "int", nullable: false),
                    MonthlyXp = table.Column<int>(type: "int", nullable: false),
                    CurrentLeague = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompleteDailyChallenge = table.Column<bool>(type: "bit", nullable: false),
                    DateCompleteDailyChallenge = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompleteWeeklyChallenge = table.Column<bool>(type: "bit", nullable: false),
                    DateCompleteWeeklyChallenge = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompleteMonthlyChallenge = table.Column<bool>(type: "bit", nullable: false),
                    DateCompleteMonthlyChallenge = table.Column<DateTime>(type: "datetime2", nullable: true),
                    JoinedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IfStreakActive = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Subject",
                columns: table => new
                {
                    SubjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumExams = table.Column<int>(type: "int", nullable: false),
                    TotalQuestions = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subject", x => x.SubjectId);
                    table.ForeignKey(
                        name: "FK_Subject_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Exam",
                columns: table => new
                {
                    ExamId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumQuestions = table.Column<int>(type: "int", nullable: true),
                    NumCorrectQuestions = table.Column<int>(type: "int", nullable: true),
                    NumWrongQuestions = table.Column<int>(type: "int", nullable: true),
                    DifficultyLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    McqQuestionsData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TfQuestionsData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeTaken = table.Column<TimeOnly>(type: "time", nullable: true),
                    TotalScore = table.Column<int>(type: "int", nullable: true),
                    XpCollected = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    SubjectId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exam", x => x.ExamId);
                    table.ForeignKey(
                        name: "FK_Exam_Subject_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subject",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Exam_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exam_SubjectId",
                table: "Exam",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Exam_UserId",
                table: "Exam",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Subject_UserId",
                table: "Subject",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "Exam");

            migrationBuilder.DropTable(
                name: "Subject");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
