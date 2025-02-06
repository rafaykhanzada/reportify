using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class ReportSaveModelAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WidgetReportData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WidgetReportData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WidgetSaveData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pLabel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pWId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pId = table.Column<int>(type: "int", nullable: false),
                    RId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WidgetSaveData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WidgetSaveData_WidgetPropertyData_pId",
                        column: x => x.pId,
                        principalTable: "WidgetPropertyData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WidgetSaveData_WidgetReportData_RId",
                        column: x => x.RId,
                        principalTable: "WidgetReportData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WidgetSaveData_pId",
                table: "WidgetSaveData",
                column: "pId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WidgetSaveData_RId",
                table: "WidgetSaveData",
                column: "RId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WidgetSaveData");

            migrationBuilder.DropTable(
                name: "WidgetReportData");
        }
    }
}
