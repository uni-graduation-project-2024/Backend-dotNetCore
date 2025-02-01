using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learntendo_backend.Migrations
{
    /// <inheritdoc />
    public partial class secu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Admin");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "Admin",
                newName: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Admin",
                newName: "AdminId");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Admin",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
