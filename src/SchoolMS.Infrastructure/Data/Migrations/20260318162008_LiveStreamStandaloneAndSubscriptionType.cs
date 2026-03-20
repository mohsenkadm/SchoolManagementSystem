using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class LiveStreamStandaloneAndSubscriptionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubscriptionType",
                table: "OnlineSubscriptionPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "LiveStreams",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "LiveStreams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Populate SubjectId from Course.SubjectId for existing LiveStreams
            migrationBuilder.Sql(@"
                UPDATE ls
                SET ls.SubjectId = c.SubjectId
                FROM LiveStreams ls
                INNER JOIN Courses c ON ls.CourseId = c.Id
                WHERE ls.CourseId IS NOT NULL AND ls.SubjectId = 0
            ");

            migrationBuilder.CreateIndex(
                name: "IX_LiveStreams_SubjectId",
                table: "LiveStreams",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_LiveStreams_Subjects_SubjectId",
                table: "LiveStreams",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiveStreams_Subjects_SubjectId",
                table: "LiveStreams");

            migrationBuilder.DropIndex(
                name: "IX_LiveStreams_SubjectId",
                table: "LiveStreams");

            migrationBuilder.DropColumn(
                name: "SubscriptionType",
                table: "OnlineSubscriptionPlans");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "LiveStreams");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "LiveStreams",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
