using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IPLAuctionSystem.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceIsSoldWithAuctionStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add AuctionStatus as nullable so it can be backfilled from the old IsSold flag
            migrationBuilder.AddColumn<int>(
                name: "AuctionStatus",
                table: "Players",
                type: "int",
                nullable: true);

            // Data conversion: IsSold + price > 0 -> Sold (1), IsSold + no price -> Unsold (2), else Pending (0)
            migrationBuilder.Sql(@"
UPDATE [Players]
SET [AuctionStatus] = CASE
    WHEN [IsSold] = 1 AND [SoldPrice] > 0 THEN 1
    WHEN [IsSold] = 1 THEN 2
    ELSE 0
END;");

            migrationBuilder.AlterColumn<int>(
                name: "AuctionStatus",
                table: "Players",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "IsSold",
                table: "Players");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSold",
                table: "Players",
                type: "bit",
                nullable: true);

            // Reverse conversion: both Sold and Unsold were 'processed' (true) under the old flag
            migrationBuilder.Sql(@"
UPDATE [Players]
SET [IsSold] = CASE WHEN [AuctionStatus] IN (1, 2) THEN 1 ELSE 0 END;");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSold",
                table: "Players",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "AuctionStatus",
                table: "Players");
        }
    }
}
