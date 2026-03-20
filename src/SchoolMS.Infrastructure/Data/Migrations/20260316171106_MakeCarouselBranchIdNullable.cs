using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeCarouselBranchIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarouselImages_Branches_BranchId",
                table: "CarouselImages");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "CarouselImages",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CarouselImages_Branches_BranchId",
                table: "CarouselImages",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarouselImages_Branches_BranchId",
                table: "CarouselImages");

            migrationBuilder.AlterColumn<int>(
                name: "BranchId",
                table: "CarouselImages",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CarouselImages_Branches_BranchId",
                table: "CarouselImages",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
