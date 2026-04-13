using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReactionLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Witness two substances fuse into a hot glow to learn how heat transforms separate elements into a single compound.");

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "Witness a metal dissolve into air and sends a frantic stream of bubbles of explosive gas.");

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 4,
                column: "Description",
                value: "Watch sodium react with water, releasing hydrogen gas and forming sodium hydroxide solution.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Heat iron and sulfur to observe how separate magnetic and yellow particles transform into a single non-magnetic black solid.");

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "Observe iron dissolving in hydrochloric acid as electrons transfer to hydrogen ions, producing iron(II) chloride and hydrogen gas.");

            migrationBuilder.UpdateData(
                table: "Topics",
                keyColumn: "Id",
                keyValue: 4,
                column: "Description",
                value: "Watch sodium react vigorously with water, releasing hydrogen gas and forming sodium hydroxide solution.");
        }
    }
}
