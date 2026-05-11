using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class editMedicalReportsmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "MedicalReports");

            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "MedicalReports");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "MedicalReports");

            migrationBuilder.CreateTable(
                name: "MedicalReportFile",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublicId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MedicalReportId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalReportFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalReportFile_MedicalReports_MedicalReportId",
                        column: x => x.MedicalReportId,
                        principalTable: "MedicalReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalReportFile_MedicalReportId",
                table: "MedicalReportFile",
                column: "MedicalReportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicalReportFile");

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "MedicalReports",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "MedicalReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "MedicalReports",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
