using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilentMoonApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCatalogRequiredRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tracks_AudioFileId",
                table: "Tracks");

            migrationBuilder.DropIndex(
                name: "IX_Categories_IconFileId",
                table: "Categories");

            migrationBuilder.AlterColumn<Guid>(
                name: "AudioFileId",
                table: "Tracks",
                type: "RAW(16)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "RAW(16)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "IconFileId",
                table: "Categories",
                type: "RAW(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "RAW(16)");

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_AudioFileId",
                table: "Tracks",
                column: "AudioFileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_IconFileId",
                table: "Categories",
                column: "IconFileId",
                unique: true,
                filter: "\"IconFileId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tracks_AudioFileId",
                table: "Tracks");

            migrationBuilder.DropIndex(
                name: "IX_Categories_IconFileId",
                table: "Categories");

            migrationBuilder.AlterColumn<Guid>(
                name: "AudioFileId",
                table: "Tracks",
                type: "RAW(16)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "RAW(16)");

            migrationBuilder.AlterColumn<Guid>(
                name: "IconFileId",
                table: "Categories",
                type: "RAW(16)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "RAW(16)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tracks_AudioFileId",
                table: "Tracks",
                column: "AudioFileId",
                unique: true,
                filter: "\"AudioFileId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_IconFileId",
                table: "Categories",
                column: "IconFileId",
                unique: true);
        }
    }
}
