using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicYearToAttendanceAndBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AcademicYearId",
                table: "StudentBehaviors",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AcademicYearId",
                table: "Attendances",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentBehaviors_AcademicYearId",
                table: "StudentBehaviors",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_AcademicYearId",
                table: "Attendances",
                column: "AcademicYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_AcademicYears_AcademicYearId",
                table: "Attendances",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentBehaviors_AcademicYears_AcademicYearId",
                table: "StudentBehaviors",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_AcademicYears_AcademicYearId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentBehaviors_AcademicYears_AcademicYearId",
                table: "StudentBehaviors");

            migrationBuilder.DropIndex(
                name: "IX_StudentBehaviors_AcademicYearId",
                table: "StudentBehaviors");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_AcademicYearId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "StudentBehaviors");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "Attendances");
        }
    }
}
