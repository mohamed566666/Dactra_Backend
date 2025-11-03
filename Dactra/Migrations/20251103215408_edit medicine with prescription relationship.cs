using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class editmedicinewithprescriptionrelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "ScheduleTables");

            migrationBuilder.CreateTable(
                name: "PrescriptionWithMedicin",
                columns: table => new
                {
                    PrescriptionId = table.Column<int>(type: "int", nullable: false),
                    MedicinesId = table.Column<int>(type: "int", nullable: false),
                    frequencyPerDay = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionWithMedicin", x => new { x.PrescriptionId, x.MedicinesId });
                    table.ForeignKey(
                        name: "FK_PrescriptionWithMedicin_Medicines_MedicinesId",
                        column: x => x.MedicinesId,
                        principalTable: "Medicines",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PrescriptionWithMedicin_Prescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescriptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionWithMedicin_MedicinesId",
                table: "PrescriptionWithMedicin",
                column: "MedicinesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrescriptionWithMedicin");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ScheduleTables",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
