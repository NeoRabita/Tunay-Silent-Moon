using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilentMoonApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ImageFiles_StoredFileName",
                table: "ImageFiles");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CategoryTypes",
                newName: "Title");

            migrationBuilder.AlterColumn<string>(
                name: "UploadedFileName",
                table: "ImageFiles",
                type: "NVARCHAR2(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "StoredFileName",
                table: "ImageFiles",
                type: "NVARCHAR2(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Extension",
                table: "ImageFiles",
                type: "NVARCHAR2(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "ImageFiles",
                type: "NVARCHAR2(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.AlterColumn<string>(
                name: "ContainerName",
                table: "ImageFiles",
                type: "NVARCHAR2(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(2000)");

            migrationBuilder.CreateTable(
                name: "AudioFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DurationSec = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ContainerName = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    StoredFileName = table.Column<string>(type: "NVARCHAR2(1024)", maxLength: 1024, nullable: false),
                    UploadedFileName = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Extension = table.Column<string>(type: "NVARCHAR2(16)", maxLength: 16, nullable: false),
                    SizeBytes = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    ContentType = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    StorageProvider = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudioFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Title = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    SubTitle = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    DurationSec = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    IsFeatured = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    CategoryId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CoverImageFileId = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Courses_ImageFiles_CoverImageFileId",
                        column: x => x.CoverImageFileId,
                        principalTable: "ImageFiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Narrators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    Slug = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Narrators", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CourseFavorites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    UserId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CourseId = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseFavorites_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CourseFavorites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Title = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: false),
                    Order = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: true),
                    CourseId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NarratorId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CoverImageFileId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    AudioFileId = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tracks_AudioFiles_AudioFileId",
                        column: x => x.AudioFileId,
                        principalTable: "AudioFiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tracks_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tracks_ImageFiles_CoverImageFileId",
                        column: x => x.CoverImageFileId,
                        principalTable: "ImageFiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tracks_Narrators_NarratorId",
                        column: x => x.NarratorId,
                        principalTable: "Narrators",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TrackProgresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    PositionSec = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Completed = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "TIMESTAMP(7) WITH TIME ZONE", nullable: false),
                    UserId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TrackId = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackProgresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackProgresses_Tracks_TrackId",
                        column: x => x.TrackId,
                        principalTable: "Tracks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TrackProgresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageFiles_ContainerName_StoredFileName",
                table: "ImageFiles",
                columns: new[] { "ContainerName", "StoredFileName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AudioFiles_ContainerName_StoredFileName",
                table: "AudioFiles",
                columns: new[] { "ContainerName", "StoredFileName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseFavorites_CourseId",
                table: "CourseFavorites",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseFavorites_UserId_CourseId",
                table: "CourseFavorites",
                columns: new[] { "UserId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CategoryId",
                table: "Courses",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CoverImageFileId",
                table: "Courses",
                column: "CoverImageFileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CreatedAt",
                table: "Courses",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_IsFeatured",
                table: "Courses",
                column: "IsFeatured");

            migrationBuilder.CreateIndex(
                name: "IX_Narrators_Slug",
                table: "Narrators",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrackProgresses_TrackId",
                table: "TrackProgresses",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_TrackProgresses_UserId_TrackId",
                table: "TrackProgresses",
                columns: new[] { "UserId", "TrackId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_AudioFileId",
                table: "Tracks",
                column: "AudioFileId",
                unique: true,
                filter: "\"AudioFileId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_CourseId",
                table: "Tracks",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_CourseId_Order",
                table: "Tracks",
                columns: new[] { "CourseId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_CoverImageFileId",
                table: "Tracks",
                column: "CoverImageFileId",
                unique: true,
                filter: "\"CoverImageFileId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_NarratorId",
                table: "Tracks",
                column: "NarratorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseFavorites");

            migrationBuilder.DropTable(
                name: "TrackProgresses");

            migrationBuilder.DropTable(
                name: "Tracks");

            migrationBuilder.DropTable(
                name: "AudioFiles");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Narrators");

            migrationBuilder.DropIndex(
                name: "IX_ImageFiles_ContainerName_StoredFileName",
                table: "ImageFiles");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "CategoryTypes",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "UploadedFileName",
                table: "ImageFiles",
                type: "NVARCHAR2(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "StoredFileName",
                table: "ImageFiles",
                type: "NVARCHAR2(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(1024)",
                oldMaxLength: 1024);

            migrationBuilder.AlterColumn<string>(
                name: "Extension",
                table: "ImageFiles",
                type: "NVARCHAR2(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(16)",
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "ImageFiles",
                type: "NVARCHAR2(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ContainerName",
                table: "ImageFiles",
                type: "NVARCHAR2(2000)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateIndex(
                name: "IX_ImageFiles_StoredFileName",
                table: "ImageFiles",
                column: "StoredFileName",
                unique: true);
        }
    }
}
