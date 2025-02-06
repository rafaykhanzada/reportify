using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueConstraintFromWidgetSaveDataAndReportId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove the unique constraint on pId
            migrationBuilder.DropIndex(
                name: "IX_WidgetSaveData_pId",
                table: "WidgetSaveData");

            migrationBuilder.CreateIndex(
                name: "IX_WidgetSaveData_pId",
                table: "WidgetSaveData",
                column: "pId"); // Recreate the index without uniqueness

            // Remove the unique constraint on RId (if it exists)
            migrationBuilder.DropIndex(
                name: "IX_WidgetSaveData_RId",
                table: "WidgetSaveData");

            migrationBuilder.CreateIndex(
                name: "IX_WidgetSaveData_RId",
                table: "WidgetSaveData",
                column: "RId"); // Recreate the index without uniqueness
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Restore the unique constraint on pId
            migrationBuilder.DropIndex(
                name: "IX_WidgetSaveData_pId",
                table: "WidgetSaveData");

            migrationBuilder.CreateIndex(
                name: "IX_WidgetSaveData_pId",
                table: "WidgetSaveData",
                column: "pId",
                unique: true); // Revert back to unique if needed

            // Restore the unique constraint on RId
            migrationBuilder.DropIndex(
                name: "IX_WidgetSaveData_RId",
                table: "WidgetSaveData");

            migrationBuilder.CreateIndex(
                name: "IX_WidgetSaveData_RId",
                table: "WidgetSaveData",
                column: "RId",
                unique: true); // Revert back to unique if needed
        }

    }
}
