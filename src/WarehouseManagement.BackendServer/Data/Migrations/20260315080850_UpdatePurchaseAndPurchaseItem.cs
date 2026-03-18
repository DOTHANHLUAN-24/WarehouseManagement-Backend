using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagement.BackendServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePurchaseAndPurchaseItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Purchases",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "PurchaseItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PurchaseItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "PurchaseItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PurchaseItems");
        }
    }
}
