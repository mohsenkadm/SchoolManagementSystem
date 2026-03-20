using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSeenCountLikeCountAndTrackingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SeenCount",
                table: "LiveStreams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "CourseVideos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeenCount",
                table: "CourseVideos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LiveStreamSeens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LiveStreamId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SeenAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_LiveStreamSeens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LiveStreamSeens_LiveStreams_LiveStreamId",
                        column: x => x.LiveStreamId,
                        principalTable: "LiveStreams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LiveStreamSeens_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LiveStreamSeens_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VideoLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseVideoId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_VideoLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoLikes_CourseVideos_CourseVideoId",
                        column: x => x.CourseVideoId,
                        principalTable: "CourseVideos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VideoLikes_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VideoLikes_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VideoSeens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseVideoId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    SeenAt = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_VideoSeens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoSeens_CourseVideos_CourseVideoId",
                        column: x => x.CourseVideoId,
                        principalTable: "CourseVideos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VideoSeens_Schools_SchoolId",
                        column: x => x.SchoolId,
                        principalTable: "Schools",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VideoSeens_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LiveStreamSeens_LiveStreamId",
                table: "LiveStreamSeens",
                column: "LiveStreamId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveStreamSeens_SchoolId",
                table: "LiveStreamSeens",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_LiveStreamSeens_StudentId",
                table: "LiveStreamSeens",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoLikes_CourseVideoId",
                table: "VideoLikes",
                column: "CourseVideoId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoLikes_SchoolId",
                table: "VideoLikes",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoLikes_StudentId",
                table: "VideoLikes",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoSeens_CourseVideoId",
                table: "VideoSeens",
                column: "CourseVideoId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoSeens_SchoolId",
                table: "VideoSeens",
                column: "SchoolId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoSeens_StudentId",
                table: "VideoSeens",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LiveStreamSeens");

            migrationBuilder.DropTable(
                name: "VideoLikes");

            migrationBuilder.DropTable(
                name: "VideoSeens");

            migrationBuilder.DropColumn(
                name: "SeenCount",
                table: "LiveStreams");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "CourseVideos");

            migrationBuilder.DropColumn(
                name: "SeenCount",
                table: "CourseVideos");
        }
    }
}
