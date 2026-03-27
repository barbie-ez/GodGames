using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GodGames.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorldEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Biome = table.Column<string>(type: "text", nullable: false),
                    StatRequirementsJson = table.Column<string>(type: "text", nullable: false),
                    OutcomeModifiersJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Champions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GodId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Class = table.Column<string>(type: "text", nullable: false),
                    stat_str = table.Column<int>(type: "integer", nullable: false),
                    stat_dex = table.Column<int>(type: "integer", nullable: false),
                    stat_int = table.Column<int>(type: "integer", nullable: false),
                    stat_wis = table.Column<int>(type: "integer", nullable: false),
                    stat_vit = table.Column<int>(type: "integer", nullable: false),
                    HP = table.Column<int>(type: "integer", nullable: false),
                    MaxHP = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    XP = table.Column<int>(type: "integer", nullable: false),
                    PowerUpSlot = table.Column<string>(type: "text", nullable: true),
                    Biome = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastTickAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Champions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Champions_Gods_GodId",
                        column: x => x.GodId,
                        principalTable: "Gods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Interventions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GodId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChampionId = table.Column<Guid>(type: "uuid", nullable: false),
                    RawCommand = table.Column<string>(type: "text", nullable: false),
                    ParsedEffectJson = table.Column<string>(type: "text", nullable: false),
                    IsApplied = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interventions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interventions_Champions_ChampionId",
                        column: x => x.ChampionId,
                        principalTable: "Champions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Interventions_Gods_GodId",
                        column: x => x.GodId,
                        principalTable: "Gods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NarrativeEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChampionId = table.Column<Guid>(type: "uuid", nullable: false),
                    TickNumber = table.Column<int>(type: "integer", nullable: false),
                    StoryText = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NarrativeEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NarrativeEntries_Champions_ChampionId",
                        column: x => x.ChampionId,
                        principalTable: "Champions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Champions_GodId",
                table: "Champions",
                column: "GodId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gods_Email",
                table: "Gods",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_ChampionId",
                table: "Interventions",
                column: "ChampionId");

            migrationBuilder.CreateIndex(
                name: "IX_Interventions_GodId_IsApplied",
                table: "Interventions",
                columns: new[] { "GodId", "IsApplied" });

            migrationBuilder.CreateIndex(
                name: "IX_NarrativeEntries_ChampionId_TickNumber",
                table: "NarrativeEntries",
                columns: new[] { "ChampionId", "TickNumber" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Interventions");

            migrationBuilder.DropTable(
                name: "NarrativeEntries");

            migrationBuilder.DropTable(
                name: "WorldEvents");

            migrationBuilder.DropTable(
                name: "Champions");

            migrationBuilder.DropTable(
                name: "Gods");
        }
    }
}
