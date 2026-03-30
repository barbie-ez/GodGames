using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GodGames.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPowerUpTicksRemaining : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PowerUpTicksRemaining",
                table: "Champions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PowerUpTicksRemaining",
                table: "Champions");
        }
    }
}
