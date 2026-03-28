using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GodGames.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint6_LuckPersonalityWorldMap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActiveDebuff",
                table: "Champions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ActiveDebuffTicksRemaining",
                table: "Champions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CombatWins",
                table: "Champions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ConsecutiveCursedTicks",
                table: "Champions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CurrentRegionId",
                table: "Champions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DivineFavourCount",
                table: "Champions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ExploredRegionIds",
                table: "Champions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PersonalityTrait",
                table: "Champions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TicksSurvivedStreak",
                table: "Champions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PatronTitle",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WorldRegions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Biome = table.Column<string>(type: "text", nullable: false),
                    DifficultyRating = table.Column<int>(type: "integer", nullable: false),
                    MinLevelRequired = table.Column<int>(type: "integer", nullable: false),
                    MapX = table.Column<int>(type: "integer", nullable: false),
                    MapY = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ActiveEventTypes = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorldRegions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "WorldRegions",
                columns: new[] { "Id", "ActiveEventTypes", "Biome", "Description", "DifficultyRating", "MapX", "MapY", "MinLevelRequired", "Name" },
                values: new object[,]
                {
                    { "ashwood-crossing", "Combat, Exploration, Curse", "Normal", "A river crossing through a charred forest, haunted by restless spirits.", 5, 200, 280, 5, "Ashwood Crossing" },
                    { "dragonspine-ridge", "Combat, Dragon, Fire", "Dangerous", "Volcanic ridgelines where dragons nest and the air tastes of sulphur.", 8, 80, 120, 10, "Dragonspine Ridge" },
                    { "greenwatch-village", "Trade, Rest, Healing", "Safe", "A welcoming village with markets, healers, and rumours of glory.", 2, 200, 510, 1, "Greenwatch Village" },
                    { "ironveil-pass", "Combat, Ambush, Trade", "Normal", "A treacherous mountain pass frequented by bandits and desperate travellers.", 4, 70, 300, 5, "Ironveil Pass" },
                    { "sunken-grove", "Exploration, Combat, Loot", "Safe", "Ancient ruins half-buried in woodland, with minor creatures lurking.", 3, 330, 460, 2, "Sunken Grove" },
                    { "the-bleached-crypt", "Combat, Loot, Undead", "Normal", "An ancient necropolis filled with undead guardians and forgotten treasure.", 6, 330, 320, 7, "The Bleached Crypt" },
                    { "the-obsidian-keep", "Combat, Boss, Necromancy", "Dangerous", "Fortress of the Necromancer-King — only the mightiest dare enter.", 10, 330, 110, 12, "The Obsidian Keep" },
                    { "void-threshold", "Combat, Demon, Void", "Dangerous", "A rift in reality where demon lords breach the mortal plane.", 9, 200, 80, 10, "Void Threshold" },
                    { "whispering-fields", "Exploration, Rest, Trade", "Safe", "Gentle grasslands where young champions first find their footing.", 1, 80, 480, 1, "Whispering Fields" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorldRegions");

            migrationBuilder.DropColumn(
                name: "ActiveDebuff",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "ActiveDebuffTicksRemaining",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "CombatWins",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "ConsecutiveCursedTicks",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "CurrentRegionId",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "DivineFavourCount",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "ExploredRegionIds",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "PersonalityTrait",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "TicksSurvivedStreak",
                table: "Champions");

            migrationBuilder.DropColumn(
                name: "PatronTitle",
                table: "AspNetUsers");
        }
    }
}
