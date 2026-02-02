using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IPLAuctionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamLogo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TeamLogo",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeamLogo",
                table: "Teams");
        }
    }
}
