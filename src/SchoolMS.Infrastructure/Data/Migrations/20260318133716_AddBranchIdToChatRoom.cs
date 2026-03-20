using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchIdToChatRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "ChatRooms",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatRooms_BranchId",
                table: "ChatRooms",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatRooms_Branches_BranchId",
                table: "ChatRooms",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatRooms_Branches_BranchId",
                table: "ChatRooms");

            migrationBuilder.DropIndex(
                name: "IX_ChatRooms_BranchId",
                table: "ChatRooms");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "ChatRooms");
        }
    }
}
