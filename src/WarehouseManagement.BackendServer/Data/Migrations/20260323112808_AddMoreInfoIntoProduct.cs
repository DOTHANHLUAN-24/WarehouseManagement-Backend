using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagement.BackendServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreInfoIntoProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "ProductVariants",
                newName: "SellingPrice");

            migrationBuilder.AddColumn<int>(
                name: "MinStockLevel",
                table: "ProductVariants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalPrice",
                table: "ProductVariants",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinStockLevel",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                table: "ProductVariants");

            migrationBuilder.RenameColumn(
                name: "SellingPrice",
                table: "ProductVariants",
                newName: "Price");
        }
    }
}
