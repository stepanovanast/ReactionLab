using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ReactionLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Badges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Badges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    AvatarId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Equation = table.Column<string>(type: "text", nullable: false),
                    MoleculeData = table.Column<string>(type: "text", nullable: false),
                    StepsData = table.Column<string>(type: "text", nullable: false),
                    Temperature = table.Column<double>(type: "double precision", nullable: false),
                    TopicId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reactions_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBadges",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    BadgeId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBadges", x => new { x.UserId, x.BadgeId });
                    table.ForeignKey(
                        name: "FK_UserBadges_Badges_BadgeId",
                        column: x => x.BadgeId,
                        principalTable: "Badges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBadges_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProgress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    TopicId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProgress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProgress_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProgress_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Badges",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Join ReactionLab", "Early Bird" },
                    { 2, "Complete the Iron Sulfide Formation reaction", "Iron Alchemist" },
                    { 3, "Complete your second topic", "Rising Chemist" },
                    { 4, "Complete your third topic", "Lab Veteran" },
                    { 5, "Complete your fourth topic", "Chemistry Master" },
                    { 6, "Switch visualization mode for the first time", "Molecular Vision" },
                    { 7, "Reach the final step of any reaction", "Full Circuit" },
                    { 8, "Complete all topics in the lab", "Completionist" }
                });

            migrationBuilder.InsertData(
                table: "Topics",
                columns: new[] { "Id", "Description", "Order", "Title" },
                values: new object[,]
                {
                    { 1, "Heat iron and sulfur to observe how separate magnetic and yellow particles transform into a single non-magnetic black solid.", 1, "Iron Sulfide Formation" },
                    { 2, "Observe iron dissolving in hydrochloric acid as electrons transfer to hydrogen ions, producing iron(II) chloride and hydrogen gas.", 2, "Production of Hydrogen Gas" },
                    { 3, "Decompose hydrogen peroxide with a catalyst and test the released oxygen using the glowing splint technique.", 3, "Glowing Splint Test" },
                    { 4, "Watch sodium react vigorously with water, releasing hydrogen gas and forming sodium hydroxide solution.", 4, "Sodium-Water Reaction" }
                });

            migrationBuilder.InsertData(
                table: "Reactions",
                columns: new[] { "Id", "Equation", "MoleculeData", "StepsData", "Temperature", "Title", "TopicId" },
                values: new object[] { 1, "8 Fe + 8 S → 8 FeS", "[]", "[\n  {\n    \"number\": 1,\n    \"title\": \"Ambient Mixture\",\n    \"description\": \"Iron (Fe) appears as grey magnetic particles arranged in a metallic lattice. Sulfur (S) forms S\\u2088 crown rings \\u2014 8 yellow atoms bonded in a circle. The two substances are physically separate; a magnet would still pull the grey particles away.\",\n    \"temperatureRange\": {\n      \"start\": 20,\n      \"end\": 115\n    },\n    \"background\": \"default\",\n    \"atoms\": [\n      {\n        \"id\": \"fe1\",\n        \"element\": \"Fe\",\n        \"x\": -3.54,\n        \"y\": -0.74,\n        \"z\": -0.74\n      },\n      {\n        \"id\": \"fe2\",\n        \"element\": \"Fe\",\n        \"x\": -2.06,\n        \"y\": -0.74,\n        \"z\": -0.74\n      },\n      {\n        \"id\": \"fe3\",\n        \"element\": \"Fe\",\n        \"x\": -3.54,\n        \"y\": -0.74,\n        \"z\": 0.74\n      },\n      {\n        \"id\": \"fe4\",\n        \"element\": \"Fe\",\n        \"x\": -2.06,\n        \"y\": -0.74,\n        \"z\": 0.74\n      },\n      {\n        \"id\": \"fe5\",\n        \"element\": \"Fe\",\n        \"x\": -3.54,\n        \"y\": 0.74,\n        \"z\": -0.74\n      },\n      {\n        \"id\": \"fe6\",\n        \"element\": \"Fe\",\n        \"x\": -2.06,\n        \"y\": 0.74,\n        \"z\": -0.74\n      },\n      {\n        \"id\": \"fe7\",\n        \"element\": \"Fe\",\n        \"x\": -3.54,\n        \"y\": 0.74,\n        \"z\": 0.74\n      },\n      {\n        \"id\": \"fe8\",\n        \"element\": \"Fe\",\n        \"x\": -2.06,\n        \"y\": 0.74,\n        \"z\": 0.74\n      },\n      {\n        \"id\": \"s1\",\n        \"element\": \"S\",\n        \"x\": 4.33,\n        \"y\": 0.27,\n        \"z\": 0\n      },\n      {\n        \"id\": \"s2\",\n        \"element\": \"S\",\n        \"x\": 3.88,\n        \"y\": -0.27,\n        \"z\": 1.08\n      },\n      {\n        \"id\": \"s3\",\n        \"element\": \"S\",\n        \"x\": 2.8,\n        \"y\": 0.27,\n        \"z\": 1.53\n      },\n      {\n        \"id\": \"s4\",\n        \"element\": \"S\",\n        \"x\": 1.72,\n        \"y\": -0.27,\n        \"z\": 1.08\n      },\n      {\n        \"id\": \"s5\",\n        \"element\": \"S\",\n        \"x\": 1.27,\n        \"y\": 0.27,\n        \"z\": -0\n      },\n      {\n        \"id\": \"s6\",\n        \"element\": \"S\",\n        \"x\": 1.72,\n        \"y\": -0.27,\n        \"z\": -1.08\n      },\n      {\n        \"id\": \"s7\",\n        \"element\": \"S\",\n        \"x\": 2.8,\n        \"y\": 0.27,\n        \"z\": -1.53\n      },\n      {\n        \"id\": \"s8\",\n        \"element\": \"S\",\n        \"x\": 3.88,\n        \"y\": -0.27,\n        \"z\": -1.08\n      }\n    ],\n    \"bonds\": [\n      {\n        \"from\": \"fe1\",\n        \"to\": \"fe2\"\n      },\n      {\n        \"from\": \"fe3\",\n        \"to\": \"fe4\"\n      },\n      {\n        \"from\": \"fe1\",\n        \"to\": \"fe3\"\n      },\n      {\n        \"from\": \"fe2\",\n        \"to\": \"fe4\"\n      },\n      {\n        \"from\": \"fe5\",\n        \"to\": \"fe6\"\n      },\n      {\n        \"from\": \"fe7\",\n        \"to\": \"fe8\"\n      },\n      {\n        \"from\": \"fe5\",\n        \"to\": \"fe7\"\n      },\n      {\n        \"from\": \"fe6\",\n        \"to\": \"fe8\"\n      },\n      {\n        \"from\": \"fe1\",\n        \"to\": \"fe5\"\n      },\n      {\n        \"from\": \"fe2\",\n        \"to\": \"fe6\"\n      },\n      {\n        \"from\": \"fe3\",\n        \"to\": \"fe7\"\n      },\n      {\n        \"from\": \"fe4\",\n        \"to\": \"fe8\"\n      },\n      {\n        \"from\": \"s1\",\n        \"to\": \"s2\"\n      },\n      {\n        \"from\": \"s2\",\n        \"to\": \"s3\"\n      },\n      {\n        \"from\": \"s3\",\n        \"to\": \"s4\"\n      },\n      {\n        \"from\": \"s4\",\n        \"to\": \"s5\"\n      },\n      {\n        \"from\": \"s5\",\n        \"to\": \"s6\"\n      },\n      {\n        \"from\": \"s6\",\n        \"to\": \"s7\"\n      },\n      {\n        \"from\": \"s7\",\n        \"to\": \"s8\"\n      },\n      {\n        \"from\": \"s8\",\n        \"to\": \"s1\"\n      }\n    ],\n    \"electronTransfers\": [],\n    \"atomOverrides\": {}\n  },\n  {\n    \"number\": 2,\n    \"title\": \"Activation and Liquefaction\",\n    \"description\": \"At 119 \\u00B0C the S\\u2088 rings break apart. Sulfur becomes fluid, surrounding and coating the iron particles. The yellow atoms flow around the stationary iron blocks \\u2014 the wetting phase.\",\n    \"temperatureRange\": {\n      \"start\": 119,\n      \"end\": 445\n    },\n    \"background\": \"default\",\n    \"atoms\": [\n      {\n        \"id\": \"fe1\",\n        \"element\": \"Fe\",\n        \"x\": -0.74,\n        \"y\": -0.74,\n        \"z\": -0.74\n      },\n      {\n        \"id\": \"fe2\",\n        \"element\": \"Fe\",\n        \"x\": 0.74,\n        \"y\": -0.74,\n        \"z\": -0.74\n      },\n      {\n        \"id\": \"fe3\",\n        \"element\": \"Fe\",\n        \"x\": -0.74,\n        \"y\": -0.74,\n        \"z\": 0.74\n      },\n      {\n        \"id\": \"fe4\",\n        \"element\": \"Fe\",\n        \"x\": 0.74,\n        \"y\": -0.74,\n        \"z\": 0.74\n      },\n      {\n        \"id\": \"fe5\",\n        \"element\": \"Fe\",\n        \"x\": -0.74,\n        \"y\": 0.74,\n        \"z\": -0.74\n      },\n      {\n        \"id\": \"fe6\",\n        \"element\": \"Fe\",\n        \"x\": 0.74,\n        \"y\": 0.74,\n        \"z\": -0.74\n      },\n      {\n        \"id\": \"fe7\",\n        \"element\": \"Fe\",\n        \"x\": -0.74,\n        \"y\": 0.74,\n        \"z\": 0.74\n      },\n      {\n        \"id\": \"fe8\",\n        \"element\": \"Fe\",\n        \"x\": 0.74,\n        \"y\": 0.74,\n        \"z\": 0.74\n      },\n      {\n        \"id\": \"s1\",\n        \"element\": \"S\",\n        \"x\": 0,\n        \"y\": 3.44,\n        \"z\": 0\n      },\n      {\n        \"id\": \"s2\",\n        \"element\": \"S\",\n        \"x\": -1.68,\n        \"y\": 2.58,\n        \"z\": 1.54\n      },\n      {\n        \"id\": \"s3\",\n        \"element\": \"S\",\n        \"x\": 0.26,\n        \"y\": 1.72,\n        \"z\": -2.97\n      },\n      {\n        \"id\": \"s4\",\n        \"element\": \"S\",\n        \"x\": 2.03,\n        \"y\": 0.86,\n        \"z\": 2.65\n      },\n      {\n        \"id\": \"s5\",\n        \"element\": \"S\",\n        \"x\": -3.39,\n        \"y\": -0,\n        \"z\": -0.6\n      },\n      {\n        \"id\": \"s6\",\n        \"element\": \"S\",\n        \"x\": 2.81,\n        \"y\": -0.86,\n        \"z\": -1.79\n      },\n      {\n        \"id\": \"s7\",\n        \"element\": \"S\",\n        \"x\": -0.77,\n        \"y\": -1.72,\n        \"z\": 2.88\n      },\n      {\n        \"id\": \"s8\",\n        \"element\": \"S\",\n        \"x\": -1.05,\n        \"y\": -2.58,\n        \"z\": -2.02\n      }\n    ],\n    \"bonds\": [\n      {\n        \"from\": \"fe1\",\n        \"to\": \"fe2\"\n      },\n      {\n        \"from\": \"fe3\",\n        \"to\": \"fe4\"\n      },\n      {\n        \"from\": \"fe1\",\n        \"to\": \"fe3\"\n      },\n      {\n        \"from\": \"fe2\",\n        \"to\": \"fe4\"\n      },\n      {\n        \"from\": \"fe5\",\n        \"to\": \"fe6\"\n      },\n      {\n        \"from\": \"fe7\",\n        \"to\": \"fe8\"\n      },\n      {\n        \"from\": \"fe5\",\n        \"to\": \"fe7\"\n      },\n      {\n        \"from\": \"fe6\",\n        \"to\": \"fe8\"\n      },\n      {\n        \"from\": \"fe1\",\n        \"to\": \"fe5\"\n      },\n      {\n        \"from\": \"fe2\",\n        \"to\": \"fe6\"\n      },\n      {\n        \"from\": \"fe3\",\n        \"to\": \"fe7\"\n      },\n      {\n        \"from\": \"fe4\",\n        \"to\": \"fe8\"\n      }\n    ],\n    \"electronTransfers\": [],\n    \"atomOverrides\": {}\n  },\n  {\n    \"number\": 3,\n    \"title\": \"Exothermic Fusion\",\n    \"description\": \"The high-energy moment: electrons leap from each iron atom to a sulfur neighbour. Iron shrinks as it becomes Fe\\u00B2\\u207A and sulfur expands as it becomes S\\u00B2\\u207B. The reaction releases ~100 kJ/mol.\",\n    \"temperatureRange\": {\n      \"start\": 600,\n      \"end\": 900\n    },\n    \"background\": \"default\",\n    \"atoms\": [\n      {\n        \"id\": \"fe1\",\n        \"element\": \"Fe\",\n        \"x\": -0.74,\n        \"y\": -0.74,\n        \"z\": -0.74\n      },\n      {\n        \"id\": \"fe2\",\n        \"element\": \"Fe\",\n        \"x\": 0.74,\n        \"y\": -0.74,\n        \"z\": -0.74\n      },\n      {\n        \"id\": \"fe3\",\n        \"element\": \"Fe\",\n        \"x\": -0.74,\n        \"y\": -0.74,\n        \"z\": 0.74\n      },\n      {\n        \"id\": \"fe4\",\n        \"element\": \"Fe\",\n        \"x\": 0.74,\n        \"y\": -0.74,\n        \"z\": 0.74\n      },\n      {\n        \"id\": \"fe5\",\n        \"element\": \"Fe\",\n        \"x\": -0.74,\n        \"y\": 0.74,\n        \"z\": -0.74\n      },\n      {\n        \"id\": \"fe6\",\n        \"element\": \"Fe\",\n        \"x\": 0.74,\n        \"y\": 0.74,\n        \"z\": -0.74\n      },\n      {\n        \"id\": \"fe7\",\n        \"element\": \"Fe\",\n        \"x\": -0.74,\n        \"y\": 0.74,\n        \"z\": 0.74\n      },\n      {\n        \"id\": \"fe8\",\n        \"element\": \"Fe\",\n        \"x\": 0.74,\n        \"y\": 0.74,\n        \"z\": 0.74\n      },\n      {\n        \"id\": \"s1\",\n        \"element\": \"S\",\n        \"x\": 0,\n        \"y\": 3.44,\n        \"z\": 0\n      },\n      {\n        \"id\": \"s2\",\n        \"element\": \"S\",\n        \"x\": -1.68,\n        \"y\": 2.58,\n        \"z\": 1.54\n      },\n      {\n        \"id\": \"s3\",\n        \"element\": \"S\",\n        \"x\": 0.26,\n        \"y\": 1.72,\n        \"z\": -2.97\n      },\n      {\n        \"id\": \"s4\",\n        \"element\": \"S\",\n        \"x\": 2.03,\n        \"y\": 0.86,\n        \"z\": 2.65\n      },\n      {\n        \"id\": \"s5\",\n        \"element\": \"S\",\n        \"x\": -3.39,\n        \"y\": -0,\n        \"z\": -0.6\n      },\n      {\n        \"id\": \"s6\",\n        \"element\": \"S\",\n        \"x\": 2.81,\n        \"y\": -0.86,\n        \"z\": -1.79\n      },\n      {\n        \"id\": \"s7\",\n        \"element\": \"S\",\n        \"x\": -0.77,\n        \"y\": -1.72,\n        \"z\": 2.88\n      },\n      {\n        \"id\": \"s8\",\n        \"element\": \"S\",\n        \"x\": -1.05,\n        \"y\": -2.58,\n        \"z\": -2.02\n      }\n    ],\n    \"bonds\": [],\n    \"electronTransfers\": [\n      {\n        \"from\": \"fe1\",\n        \"to\": \"s1\"\n      },\n      {\n        \"from\": \"fe2\",\n        \"to\": \"s2\"\n      },\n      {\n        \"from\": \"fe3\",\n        \"to\": \"s3\"\n      },\n      {\n        \"from\": \"fe4\",\n        \"to\": \"s4\"\n      },\n      {\n        \"from\": \"fe5\",\n        \"to\": \"s5\"\n      },\n      {\n        \"from\": \"fe6\",\n        \"to\": \"s6\"\n      },\n      {\n        \"from\": \"fe7\",\n        \"to\": \"s7\"\n      },\n      {\n        \"from\": \"fe8\",\n        \"to\": \"s8\"\n      }\n    ],\n    \"atomOverrides\": {\n      \"fe1\": {\n        \"radiusScale\": 0.8,\n        \"dimmed\": true\n      },\n      \"fe2\": {\n        \"radiusScale\": 0.8,\n        \"dimmed\": true\n      },\n      \"fe3\": {\n        \"radiusScale\": 0.8,\n        \"dimmed\": true\n      },\n      \"fe4\": {\n        \"radiusScale\": 0.8,\n        \"dimmed\": true\n      },\n      \"fe5\": {\n        \"radiusScale\": 0.8,\n        \"dimmed\": true\n      },\n      \"fe6\": {\n        \"radiusScale\": 0.8,\n        \"dimmed\": true\n      },\n      \"fe7\": {\n        \"radiusScale\": 0.8,\n        \"dimmed\": true\n      },\n      \"fe8\": {\n        \"radiusScale\": 0.8,\n        \"dimmed\": true\n      },\n      \"s1\": {\n        \"radiusScale\": 1.2\n      },\n      \"s2\": {\n        \"radiusScale\": 1.2\n      },\n      \"s3\": {\n        \"radiusScale\": 1.2\n      },\n      \"s4\": {\n        \"radiusScale\": 1.2\n      },\n      \"s5\": {\n        \"radiusScale\": 1.2\n      },\n      \"s6\": {\n        \"radiusScale\": 1.2\n      },\n      \"s7\": {\n        \"radiusScale\": 1.2\n      },\n      \"s8\": {\n        \"radiusScale\": 1.2\n      }\n    }\n  },\n  {\n    \"number\": 4,\n    \"title\": \"Lattice Solidification\",\n    \"description\": \"The chaos stops. Fe\\u00B2\\u207A and S\\u00B2\\u207B ions snap into a rigid NiAs-type crystal lattice \\u2014 each iron atom locked between sulfur neighbours above and below. The glow fades into a dull charcoal-black solid: non-magnetic, brittle, and complete.\",\n    \"temperatureRange\": {\n      \"start\": 900,\n      \"end\": 25\n    },\n    \"background\": \"default\",\n    \"atoms\": [\n      {\n        \"id\": \"s1\",\n        \"element\": \"S\",\n        \"x\": 0,\n        \"y\": -1.32,\n        \"z\": -1.03\n      },\n      {\n        \"id\": \"s2\",\n        \"element\": \"S\",\n        \"x\": 1.03,\n        \"y\": -1.32,\n        \"z\": 0\n      },\n      {\n        \"id\": \"s3\",\n        \"element\": \"S\",\n        \"x\": 0,\n        \"y\": -1.32,\n        \"z\": 1.03\n      },\n      {\n        \"id\": \"s4\",\n        \"element\": \"S\",\n        \"x\": -1.03,\n        \"y\": -1.32,\n        \"z\": 0\n      },\n      {\n        \"id\": \"fe1\",\n        \"element\": \"Fe\",\n        \"x\": -1.03,\n        \"y\": -0.44,\n        \"z\": -1.03\n      },\n      {\n        \"id\": \"fe2\",\n        \"element\": \"Fe\",\n        \"x\": 1.03,\n        \"y\": -0.44,\n        \"z\": -1.03\n      },\n      {\n        \"id\": \"fe3\",\n        \"element\": \"Fe\",\n        \"x\": -1.03,\n        \"y\": -0.44,\n        \"z\": 1.03\n      },\n      {\n        \"id\": \"fe4\",\n        \"element\": \"Fe\",\n        \"x\": 1.03,\n        \"y\": -0.44,\n        \"z\": 1.03\n      },\n      {\n        \"id\": \"s5\",\n        \"element\": \"S\",\n        \"x\": 0,\n        \"y\": 0.44,\n        \"z\": -1.03\n      },\n      {\n        \"id\": \"s6\",\n        \"element\": \"S\",\n        \"x\": 1.03,\n        \"y\": 0.44,\n        \"z\": 0\n      },\n      {\n        \"id\": \"s7\",\n        \"element\": \"S\",\n        \"x\": 0,\n        \"y\": 0.44,\n        \"z\": 1.03\n      },\n      {\n        \"id\": \"s8\",\n        \"element\": \"S\",\n        \"x\": -1.03,\n        \"y\": 0.44,\n        \"z\": 0\n      },\n      {\n        \"id\": \"fe5\",\n        \"element\": \"Fe\",\n        \"x\": -1.03,\n        \"y\": 1.32,\n        \"z\": -1.03\n      },\n      {\n        \"id\": \"fe6\",\n        \"element\": \"Fe\",\n        \"x\": 1.03,\n        \"y\": 1.32,\n        \"z\": -1.03\n      },\n      {\n        \"id\": \"fe7\",\n        \"element\": \"Fe\",\n        \"x\": -1.03,\n        \"y\": 1.32,\n        \"z\": 1.03\n      },\n      {\n        \"id\": \"fe8\",\n        \"element\": \"Fe\",\n        \"x\": 1.03,\n        \"y\": 1.32,\n        \"z\": 1.03\n      }\n    ],\n    \"bonds\": [\n      {\n        \"from\": \"fe1\",\n        \"to\": \"s1\"\n      },\n      {\n        \"from\": \"fe1\",\n        \"to\": \"s4\"\n      },\n      {\n        \"from\": \"fe2\",\n        \"to\": \"s1\"\n      },\n      {\n        \"from\": \"fe2\",\n        \"to\": \"s2\"\n      },\n      {\n        \"from\": \"fe3\",\n        \"to\": \"s3\"\n      },\n      {\n        \"from\": \"fe3\",\n        \"to\": \"s4\"\n      },\n      {\n        \"from\": \"fe4\",\n        \"to\": \"s2\"\n      },\n      {\n        \"from\": \"fe4\",\n        \"to\": \"s3\"\n      },\n      {\n        \"from\": \"fe1\",\n        \"to\": \"s5\"\n      },\n      {\n        \"from\": \"fe1\",\n        \"to\": \"s8\"\n      },\n      {\n        \"from\": \"fe2\",\n        \"to\": \"s5\"\n      },\n      {\n        \"from\": \"fe2\",\n        \"to\": \"s6\"\n      },\n      {\n        \"from\": \"fe3\",\n        \"to\": \"s7\"\n      },\n      {\n        \"from\": \"fe3\",\n        \"to\": \"s8\"\n      },\n      {\n        \"from\": \"fe4\",\n        \"to\": \"s6\"\n      },\n      {\n        \"from\": \"fe4\",\n        \"to\": \"s7\"\n      },\n      {\n        \"from\": \"fe5\",\n        \"to\": \"s5\"\n      },\n      {\n        \"from\": \"fe5\",\n        \"to\": \"s8\"\n      },\n      {\n        \"from\": \"fe6\",\n        \"to\": \"s5\"\n      },\n      {\n        \"from\": \"fe6\",\n        \"to\": \"s6\"\n      },\n      {\n        \"from\": \"fe7\",\n        \"to\": \"s7\"\n      },\n      {\n        \"from\": \"fe7\",\n        \"to\": \"s8\"\n      },\n      {\n        \"from\": \"fe8\",\n        \"to\": \"s6\"\n      },\n      {\n        \"from\": \"fe8\",\n        \"to\": \"s7\"\n      }\n    ],\n    \"electronTransfers\": [],\n    \"atomOverrides\": {\n      \"fe1\": {\n        \"radiusScale\": 0.8,\n        \"darkened\": true\n      },\n      \"fe2\": {\n        \"radiusScale\": 0.8,\n        \"darkened\": true\n      },\n      \"fe3\": {\n        \"radiusScale\": 0.8,\n        \"darkened\": true\n      },\n      \"fe4\": {\n        \"radiusScale\": 0.8,\n        \"darkened\": true\n      },\n      \"fe5\": {\n        \"radiusScale\": 0.8,\n        \"darkened\": true\n      },\n      \"fe6\": {\n        \"radiusScale\": 0.8,\n        \"darkened\": true\n      },\n      \"fe7\": {\n        \"radiusScale\": 0.8,\n        \"darkened\": true\n      },\n      \"fe8\": {\n        \"radiusScale\": 0.8,\n        \"darkened\": true\n      },\n      \"s1\": {\n        \"radiusScale\": 1.2,\n        \"darkened\": true\n      },\n      \"s2\": {\n        \"radiusScale\": 1.2,\n        \"darkened\": true\n      },\n      \"s3\": {\n        \"radiusScale\": 1.2,\n        \"darkened\": true\n      },\n      \"s4\": {\n        \"radiusScale\": 1.2,\n        \"darkened\": true\n      },\n      \"s5\": {\n        \"radiusScale\": 1.2,\n        \"darkened\": true\n      },\n      \"s6\": {\n        \"radiusScale\": 1.2,\n        \"darkened\": true\n      },\n      \"s7\": {\n        \"radiusScale\": 1.2,\n        \"darkened\": true\n      },\n      \"s8\": {\n        \"radiusScale\": 1.2,\n        \"darkened\": true\n      }\n    }\n  }\n]", 900.0, "Iron Sulfide Formation", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_TopicId",
                table: "Reactions",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBadges_BadgeId",
                table: "UserBadges",
                column: "BadgeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProgress_TopicId",
                table: "UserProgress",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProgress_UserId_TopicId",
                table: "UserProgress",
                columns: new[] { "UserId", "TopicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reactions");

            migrationBuilder.DropTable(
                name: "UserBadges");

            migrationBuilder.DropTable(
                name: "UserProgress");

            migrationBuilder.DropTable(
                name: "Badges");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
