using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPromoCodeSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "StudentSubscriptions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalAmount",
                table: "StudentSubscriptions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "PromoCodes",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "PromoCodes",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PromoCodes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnlimited",
                table: "PromoCodes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PromoCodeUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PromoCodeId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudentSubscriptionId = table.Column<int>(type: "int", nullable: true),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_PromoCodeUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromoCodeUsages_PromoCodes_PromoCodeId",
                        column: x => x.PromoCodeId,
                        principalTable: "PromoCodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PromoCodeUsages_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PromoCodeUsages_StudentSubscriptions_StudentSubscriptionId",
                        column: x => x.StudentSubscriptionId,
                        principalTable: "StudentSubscriptions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PromoCodeUsages_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodes_Code",
                table: "PromoCodes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeUsages_PromoCodeId",
                table: "PromoCodeUsages",
                column: "PromoCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeUsages_SchoolId",
                table: "PromoCodeUsages",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeUsages_StudentId",
                table: "PromoCodeUsages",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodeUsages_StudentSubscriptionId",
                table: "PromoCodeUsages",
                column: "StudentSubscriptionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromoCodeUsages");

            migrationBuilder.DropIndex(
                name: "IX_PromoCodes_Code",
                table: "PromoCodes");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "StudentSubscriptions");

            migrationBuilder.DropColumn(
                name: "OriginalAmount",
                table: "StudentSubscriptions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PromoCodes");

            migrationBuilder.DropColumn(
                name: "IsUnlimited",
                table: "PromoCodes");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "PromoCodes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "PromoCodes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
