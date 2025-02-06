using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class ChangedKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WidgetSaveData_WidgetPropertyData_pId",
                table: "WidgetSaveData");

            migrationBuilder.AddForeignKey(
                name: "FK_WidgetSaveData_WidgetProperty_pId",
                table: "WidgetSaveData",
                column: "pId",
                principalTable: "WidgetProperty",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WidgetSaveData_WidgetProperty_pId",
                table: "WidgetSaveData");

            migrationBuilder.AddForeignKey(
                name: "FK_WidgetSaveData_WidgetPropertyData_pId",
                table: "WidgetSaveData",
                column: "pId",
                principalTable: "WidgetPropertyData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
