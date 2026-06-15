using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagement.BackendServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseLocationToPurchaseItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Disabled: This is an obsolete SQL Server migration that ran before InitialCreate.
            // Since we migrated to PostgreSQL, InitialCreate handles table creation and a later
            // migration handles adding WarehouseLocation using character varying(200).
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Disabled: No-op
        }
    }
}
