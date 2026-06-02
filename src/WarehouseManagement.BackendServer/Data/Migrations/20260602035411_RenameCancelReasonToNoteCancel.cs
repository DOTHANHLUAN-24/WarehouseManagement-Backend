using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagement.BackendServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameCancelReasonToNoteCancel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CancelReason",
                table: "Purchases",
                newName: "NoteCancel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NoteCancel",
                table: "Purchases",
                newName: "CancelReason");
        }
    }
}
