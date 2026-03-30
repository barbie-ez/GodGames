using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GodGames.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedWorldEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "WorldEvents",
                columns: new[] { "Id", "Biome", "Description", "Name", "OutcomeModifiersJson", "StatRequirementsJson" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "Safe", "A bustling market where merchants hawk their wares and travelers share tales.", "Village Market", "{}", "{}" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "Safe", "A well-worn trail through a peaceful woodland.", "Forest Path", "{}", "{}" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "Safe", "A warm inn offering rest and rumours.", "Roadside Inn", "{}", "{}" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "Safe", "A sacred road walked by devout pilgrims seeking distant shrines.", "Pilgrim's Road", "{}", "{}" },
                    { new Guid("10000000-0000-0000-0000-000000000005"), "Safe", "A trading caravan offering exotic goods from distant lands.", "Merchant Caravan", "{}", "{}" },
                    { new Guid("10000000-0000-0000-0000-000000000006"), "Safe", "Crumbling ruins of a forgotten civilisation, now largely explored and safe.", "Ancient Ruins", "{}", "{\"WIS\": 8}" },
                    { new Guid("10000000-0000-0000-0000-000000000007"), "Safe", "A mystical spring with restorative waters blessed by forgotten gods.", "Healing Spring", "{}", "{}" },
                    { new Guid("20000000-0000-0000-0000-000000000001"), "Normal", "A gang of goblins leaps from the treeline to harass travellers.", "Goblin Ambush", "{}", "{\"STR\": 12}" },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "Normal", "A fortified camp of outlaws blocking the road ahead.", "Bandit Camp", "{}", "{\"DEX\": 12}" },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "Normal", "An ancient wood where malevolent spirits lead travellers astray.", "Dark Forest", "{}", "{\"WIS\": 12}" },
                    { new Guid("20000000-0000-0000-0000-000000000004"), "Normal", "An old burial chamber whose undead residents resent intruders.", "Haunted Crypt", "{}", "{\"INT\": 12}" },
                    { new Guid("20000000-0000-0000-0000-000000000005"), "Normal", "A treacherous mountain crossing battered by harsh winds.", "Mountain Pass", "{}", "{\"VIT\": 12}" },
                    { new Guid("20000000-0000-0000-0000-000000000006"), "Normal", "A swollen river where the current threatens to sweep travellers away.", "River Crossing", "{}", "{\"STR\": 10}" },
                    { new Guid("20000000-0000-0000-0000-000000000007"), "Normal", "A hulking troll demands payment — or combat — to cross its bridge.", "Troll Bridge", "{}", "{\"STR\": 14}" },
                    { new Guid("30000000-0000-0000-0000-000000000001"), "Dangerous", "The lair of an ancient dragon hoarding centuries of plunder.", "Dragon's Lair", "{}", "{\"STR\": 18}" },
                    { new Guid("30000000-0000-0000-0000-000000000002"), "Dangerous", "A dark spire where a lich commands an army of the undead.", "Necromancer's Tower", "{}", "{\"INT\": 18}" },
                    { new Guid("30000000-0000-0000-0000-000000000003"), "Dangerous", "The shadow compound of a deadly guild who silence loose ends.", "Assassin's Guild", "{}", "{\"DEX\": 18}" },
                    { new Guid("30000000-0000-0000-0000-000000000004"), "Dangerous", "An ancient demon of immense power seeking a soul to devour.", "Elder Demon", "{}", "{\"VIT\": 18}" },
                    { new Guid("30000000-0000-0000-0000-000000000005"), "Dangerous", "A fortress built by giants and still guarded by their descendants.", "Titan's Keep", "{}", "{\"STR\": 15, \"VIT\": 15, \"WIS\": 15}" },
                    { new Guid("30000000-0000-0000-0000-000000000006"), "Dangerous", "A tear in reality that draws the unwary into the consuming void.", "Void Rift", "{}", "{\"WIS\": 18}" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "WorldEvents",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "WorldEvents",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "WorldEvents",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "WorldEvents",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "WorldEvents",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "WorldEvents",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "WorldEvents",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "WorldEvents",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "WorldEvents",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "WorldEvents",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "WorldEvents",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "WorldEvents",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "WorldEvents",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "WorldEvents",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "WorldEvents",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "WorldEvents",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "WorldEvents",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "WorldEvents",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "WorldEvents",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "WorldEvents",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000006"));
        }
    }
}
