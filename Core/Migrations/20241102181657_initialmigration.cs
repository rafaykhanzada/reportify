using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class initialmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Widgets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DragImage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Widgets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WidgetSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WidgetSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WidgetProperty",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pLabel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Datasource = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WidgetProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WidgetProperty_WidgetSettings_WsId",
                        column: x => x.WsId,
                        principalTable: "WidgetSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WidgetWidgetSettings",
                columns: table => new
                {
                    WidgetSettingsId = table.Column<int>(type: "int", nullable: false),
                    WidgetsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WidgetWidgetSettings", x => new { x.WidgetSettingsId, x.WidgetsId });
                    table.ForeignKey(
                        name: "FK_WidgetWidgetSettings_WidgetSettings_WidgetSettingsId",
                        column: x => x.WidgetSettingsId,
                        principalTable: "WidgetSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WidgetWidgetSettings_Widgets_WidgetsId",
                        column: x => x.WidgetsId,
                        principalTable: "Widgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WidgetPropertyData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DefaultValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    propId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WidgetPropertyData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WidgetPropertyData_WidgetProperty_propId",
                        column: x => x.propId,
                        principalTable: "WidgetProperty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WidgetProperty_WsId",
                table: "WidgetProperty",
                column: "WsId");

            migrationBuilder.CreateIndex(
                name: "IX_WidgetPropertyData_propId",
                table: "WidgetPropertyData",
                column: "propId");

            migrationBuilder.CreateIndex(
                name: "IX_WidgetWidgetSettings_WidgetsId",
                table: "WidgetWidgetSettings",
                column: "WidgetsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WidgetPropertyData");

            migrationBuilder.DropTable(
                name: "WidgetWidgetSettings");

            migrationBuilder.DropTable(
                name: "WidgetProperty");

            migrationBuilder.DropTable(
                name: "Widgets");

            migrationBuilder.DropTable(
                name: "WidgetSettings");
        }
    }
}
