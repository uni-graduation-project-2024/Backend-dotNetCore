using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learntendo_backend.Migrations
{
    /// <inheritdoc />
    public partial class changeadm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Admin",
                newName: "AdminId");

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "Admin",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "Admin",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Admin",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Admin");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Admin");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Admin");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "Admin",
                newName: "UserId");
        }
    }
}
