using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStoragePlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StoragePlanId",
                table: "StorageRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StoragePlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StorageGB = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SchoolId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoragePlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoragePlans_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StorageRequests_StoragePlanId",
                table: "StorageRequests",
                column: "StoragePlanId");

            migrationBuilder.CreateIndex(
                name: "IX_StoragePlans_SchoolId",
                table: "StoragePlans",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_StorageRequests_StoragePlans_StoragePlanId",
                table: "StorageRequests",
                column: "StoragePlanId",
                principalTable: "StoragePlans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StorageRequests_StoragePlans_StoragePlanId",
                table: "StorageRequests");

            migrationBuilder.DropTable(
                name: "StoragePlans");

            migrationBuilder.DropIndex(
                name: "IX_StorageRequests_StoragePlanId",
                table: "StorageRequests");

            migrationBuilder.DropColumn(
                name: "StoragePlanId",
                table: "StorageRequests");
        }
    }
}
