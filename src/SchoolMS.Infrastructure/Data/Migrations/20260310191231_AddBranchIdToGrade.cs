using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchIdToGrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "Grades",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Grades_BranchId",
                table: "Grades",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Branches_BranchId",
                table: "Grades",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Branches_BranchId",
                table: "Grades");

            migrationBuilder.DropIndex(
                name: "IX_Grades_BranchId",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "Grades");
        }
    }
}
