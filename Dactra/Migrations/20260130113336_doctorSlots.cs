using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class doctorSlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientAppointments_ScheduleTables_ScheduleTableId",
                table: "PatientAppointments");

            migrationBuilder.DropTable(
                name: "ScheduleTables");

            migrationBuilder.RenameColumn(
                name: "ScheduleTableId",
                table: "PatientAppointments",
                newName: "SlotId");

            migrationBuilder.RenameIndex(
                name: "IX_PatientAppointments_ScheduleTableId",
                table: "PatientAppointments",
                newName: "IX_PatientAppointments_SlotId");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "PatientAppointments",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.CreateTable(
                name: "DoctorAvailabilitySlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    SlotDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsOpenByDoctor = table.Column<bool>(type: "bit", nullable: false),
                    IsBooked = table.Column<bool>(type: "bit", nullable: false),
                    AppointmentId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DoctorProfileId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorAvailabilitySlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorAvailabilitySlots_DoctorProfiles_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "DoctorProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorAvailabilitySlots_DoctorId",
                table: "DoctorAvailabilitySlots",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAppointments_DoctorAvailabilitySlots_SlotId",
                table: "PatientAppointments",
                column: "SlotId",
                principalTable: "DoctorAvailabilitySlots",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientAppointments_DoctorAvailabilitySlots_SlotId",
                table: "PatientAppointments");

            migrationBuilder.DropTable(
                name: "DoctorAvailabilitySlots");

            migrationBuilder.RenameColumn(
                name: "SlotId",
                table: "PatientAppointments",
                newName: "ScheduleTableId");

            migrationBuilder.RenameIndex(
                name: "IX_PatientAppointments_SlotId",
                table: "PatientAppointments",
                newName: "IX_PatientAppointments_ScheduleTableId");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "PatientAppointments",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "ScheduleTables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScheduleTables_DoctorProfiles_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "DoctorProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTables_DoctorId",
                table: "ScheduleTables",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAppointments_ScheduleTables_ScheduleTableId",
                table: "PatientAppointments",
                column: "ScheduleTableId",
                principalTable: "ScheduleTables",
                principalColumn: "Id");
        }
    }
}
