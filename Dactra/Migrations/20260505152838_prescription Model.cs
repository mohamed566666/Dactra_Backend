using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class prescriptionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientAppointments_Prescriptions_PrescriptionId",
                table: "PatientAppointments");

            migrationBuilder.DropTable(
                name: "MedicinesPrescription");

            migrationBuilder.DropTable(
                name: "PrescriptionWithMedicin");

            migrationBuilder.DropTable(
                name: "Medicines");

            migrationBuilder.DropIndex(
                name: "IX_PatientAppointments_PrescriptionId",
                table: "PatientAppointments");

            migrationBuilder.AlterColumn<string>(
                name: "Diagnosis",
                table: "Prescriptions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "AppointmentId",
                table: "Prescriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Prescriptions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Prescriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PrescriptionMedicines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrescriptionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Dose = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Duration = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TimesPerDay = table.Column<int>(type: "int", nullable: false),
                    WhenToTake = table.Column<int>(type: "int", nullable: false),
                    FirstDoseTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionMedicines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrescriptionMedicines_Prescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicineDoseTimes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrescriptionMedicineId = table.Column<int>(type: "int", nullable: false),
                    DoseTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    DoseOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineDoseTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicineDoseTimes_PrescriptionMedicines_PrescriptionMedicineId",
                        column: x => x.PrescriptionMedicineId,
                        principalTable: "PrescriptionMedicines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_AppointmentId",
                table: "Prescriptions",
                column: "AppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicineDoseTimes_PrescriptionMedicineId",
                table: "MedicineDoseTimes",
                column: "PrescriptionMedicineId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionMedicines_PrescriptionId",
                table: "PrescriptionMedicines",
                column: "PrescriptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_PatientAppointments_AppointmentId",
                table: "Prescriptions",
                column: "AppointmentId",
                principalTable: "PatientAppointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_PatientAppointments_AppointmentId",
                table: "Prescriptions");

            migrationBuilder.DropTable(
                name: "MedicineDoseTimes");

            migrationBuilder.DropTable(
                name: "PrescriptionMedicines");

            migrationBuilder.DropIndex(
                name: "IX_Prescriptions_AppointmentId",
                table: "Prescriptions");

            

            migrationBuilder.DropColumn(
                name: "AppointmentId",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Prescriptions");

            migrationBuilder.AlterColumn<string>(
                name: "Diagnosis",
                table: "Prescriptions",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.CreateTable(
                name: "Medicines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Indication = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PatientProfileId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medicines_Patients_PatientProfileId",
                        column: x => x.PatientProfileId,
                        principalTable: "Patients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MedicinesPrescription",
                columns: table => new
                {
                    MedicinesId = table.Column<int>(type: "int", nullable: false),
                    prescriptionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicinesPrescription", x => new { x.MedicinesId, x.prescriptionsId });
                    table.ForeignKey(
                        name: "FK_MedicinesPrescription_Medicines_MedicinesId",
                        column: x => x.MedicinesId,
                        principalTable: "Medicines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicinesPrescription_Prescriptions_prescriptionsId",
                        column: x => x.prescriptionsId,
                        principalTable: "Prescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionWithMedicin",
                columns: table => new
                {
                    PrescriptionId = table.Column<int>(type: "int", nullable: false),
                    MedicinesId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    frequencyPerDay = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionWithMedicin", x => new { x.PrescriptionId, x.MedicinesId });
                    table.ForeignKey(
                        name: "FK_PrescriptionWithMedicin_Medicines_MedicinesId",
                        column: x => x.MedicinesId,
                        principalTable: "Medicines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrescriptionWithMedicin_Prescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "Prescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientAppointments_PrescriptionId",
                table: "PatientAppointments",
                column: "PrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicines_PatientProfileId",
                table: "Medicines",
                column: "PatientProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicinesPrescription_prescriptionsId",
                table: "MedicinesPrescription",
                column: "prescriptionsId");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionWithMedicin_MedicinesId",
                table: "PrescriptionWithMedicin",
                column: "MedicinesId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAppointments_Prescriptions_PrescriptionId",
                table: "PatientAppointments",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id");
        }
    }
}
