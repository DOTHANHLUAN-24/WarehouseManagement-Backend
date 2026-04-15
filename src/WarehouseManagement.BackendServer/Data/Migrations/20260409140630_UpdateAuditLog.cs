using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagement.BackendServer.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EntityId_Temp",
                table: "AuditLogs",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.Sql(@"
        UPDATE AuditLogs
        SET EntityId_Temp = CAST(EntityId AS nvarchar(36))
    ");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "EntityId_Temp",
                table: "AuditLogs",
                newName: "EntityId");

            migrationBuilder.AlterColumn<string>(
                name: "EntityId",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(36)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Thêm lại cột GUID
            migrationBuilder.AddColumn<Guid>(
                name: "EntityId_Temp",
                table: "AuditLogs",
                type: "uniqueidentifier",
                nullable: true);

            // 2. Convert string -> GUID (chỉ convert được cái hợp lệ)
            migrationBuilder.Sql(@"
        UPDATE AuditLogs
        SET EntityId_Temp = TRY_CAST(EntityId AS uniqueidentifier)
    ");

            // 3. Xoá cột string
            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "AuditLogs");

            // 4. Rename lại
            migrationBuilder.RenameColumn(
                name: "EntityId_Temp",
                table: "AuditLogs",
                newName: "EntityId");
        }
    }
}
