using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReactionLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCuriousMindDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 8,
                column: "Description",
                value: "Revisit a reaction you've already completed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Badges",
                keyColumn: "Id",
                keyValue: 8,
                column: "Description",
                value: "View every step of a reaction at least once");
        }
    }
}
