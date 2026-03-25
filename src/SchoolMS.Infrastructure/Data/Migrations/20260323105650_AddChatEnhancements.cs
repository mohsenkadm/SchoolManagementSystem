using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddChatEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "ChatRooms",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderName",
                table: "ChatMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_TeacherId",
                table: "ChatRooms",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRooms_Teachers_TeacherId",
                table: "ChatRooms",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRooms_Teachers_TeacherId",
                table: "ChatRooms");

            migrationBuilder.DropIndex(
                name: "IX_ChatRooms_TeacherId",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "SenderName",
                table: "ChatMessages");
        }
    }
}
