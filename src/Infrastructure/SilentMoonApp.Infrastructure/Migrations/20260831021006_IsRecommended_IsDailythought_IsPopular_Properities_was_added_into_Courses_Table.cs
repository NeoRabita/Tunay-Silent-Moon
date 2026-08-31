using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SilentMoonApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IsRecommended_IsDailythought_IsPopular_Properities_was_added_into_Courses_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDailyThought",
                table: "Courses",
                type: "BOOLEAN",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPopular",
                table: "Courses",
                type: "BOOLEAN",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRecommended",
                table: "Courses",
                type: "BOOLEAN",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_IsDailyThought",
                table: "Courses",
                column: "IsDailyThought");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_IsPopular",
                table: "Courses",
                column: "IsPopular");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_IsRecommended",
                table: "Courses",
                column: "IsRecommended");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Courses_IsDailyThought",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_IsPopular",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_IsRecommended",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "IsDailyThought",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "IsPopular",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "IsRecommended",
                table: "Courses");
        }
    }
}
