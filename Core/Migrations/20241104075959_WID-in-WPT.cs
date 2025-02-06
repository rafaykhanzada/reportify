using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class WIDinWPT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "pValue",
                table: "WidgetProperty",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "WidgetId",
                table: "WidgetProperty",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WidgetProperty_WidgetId",
                table: "WidgetProperty",
                column: "WidgetId");

            migrationBuilder.AddForeignKey(
                name: "FK_WidgetProperty_Widgets_WidgetId",
                table: "WidgetProperty",
                column: "WidgetId",
                principalTable: "Widgets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WidgetProperty_Widgets_WidgetId",
                table: "WidgetProperty");

            migrationBuilder.DropIndex(
                name: "IX_WidgetProperty_WidgetId",
                table: "WidgetProperty");

            migrationBuilder.DropColumn(
                name: "WidgetId",
                table: "WidgetProperty");

            migrationBuilder.AlterColumn<string>(
                name: "pValue",
                table: "WidgetProperty",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
