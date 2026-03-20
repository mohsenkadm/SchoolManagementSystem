using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduledVideosAndStorageManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IncludesLiveStream",
                table: "SystemSubscriptionPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "StorageLimitGB",
                table: "SystemSubscriptionPlans",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ExtraStorageGB",
                table: "SchoolSubscriptions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ExtraStoragePrice",
                table: "SchoolSubscriptions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "FileSizeBytes",
                table: "CourseVideos",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsScheduled",
                table: "CourseVideos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledPublishAt",
                table: "CourseVideos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailUrl",
                table: "CourseVideos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StorageRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchoolSubscriptionId = table.Column<int>(type: "int", nullable: false),
                    RequestedGB = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PricePerGB = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_StorageRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorageRequests_SchoolSubscriptions_SchoolSubscriptionId",
                        column: x => x.SchoolSubscriptionId,
                        principalTable: "SchoolSubscriptions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StorageRequests_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StorageRequests_SchoolId",
                table: "StorageRequests",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_StorageRequests_SchoolSubscriptionId",
                table: "StorageRequests",
                column: "SchoolSubscriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StorageRequests");

            migrationBuilder.DropColumn(
                name: "IncludesLiveStream",
                table: "SystemSubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "StorageLimitGB",
                table: "SystemSubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "ExtraStorageGB",
                table: "SchoolSubscriptions");

            migrationBuilder.DropColumn(
                name: "ExtraStoragePrice",
                table: "SchoolSubscriptions");

            migrationBuilder.DropColumn(
                name: "FileSizeBytes",
                table: "CourseVideos");

            migrationBuilder.DropColumn(
                name: "IsScheduled",
                table: "CourseVideos");

            migrationBuilder.DropColumn(
                name: "ScheduledPublishAt",
                table: "CourseVideos");

            migrationBuilder.DropColumn(
                name: "ThumbnailUrl",
                table: "CourseVideos");
        }
    }
}
