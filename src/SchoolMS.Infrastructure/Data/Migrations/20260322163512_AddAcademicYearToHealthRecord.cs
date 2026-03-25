using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicYearToHealthRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AcademicYearId",
                table: "HealthRecords",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HealthRecords_AcademicYearId",
                table: "HealthRecords",
                column: "AcademicYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_HealthRecords_AcademicYears_AcademicYearId",
                table: "HealthRecords",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HealthRecords_AcademicYears_AcademicYearId",
                table: "HealthRecords");

            migrationBuilder.DropIndex(
                name: "IX_HealthRecords_AcademicYearId",
                table: "HealthRecords");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "HealthRecords");
        }
    }
}
