using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLiveStreamChatEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "LiveStreamComments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "SenderName",
                table: "LiveStreamComments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SenderType",
                table: "LiveStreamComments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "LiveStreamComments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LiveStreamComments_TeacherId",
                table: "LiveStreamComments",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_LiveStreamComments_Teachers_TeacherId",
                table: "LiveStreamComments",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiveStreamComments_Teachers_TeacherId",
                table: "LiveStreamComments");

            migrationBuilder.DropIndex(
                name: "IX_LiveStreamComments_TeacherId",
                table: "LiveStreamComments");

            migrationBuilder.DropColumn(
                name: "SenderName",
                table: "LiveStreamComments");

            migrationBuilder.DropColumn(
                name: "SenderType",
                table: "LiveStreamComments");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "LiveStreamComments");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "LiveStreamComments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
