using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IPLAuctionSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddProfilePictureToPlayer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "Players",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "Players");
        }
    }
}
