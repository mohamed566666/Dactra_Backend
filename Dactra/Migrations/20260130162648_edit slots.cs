using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class editslots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM DoctorAvailabilitySlots");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientAppointments_DoctorAvailabilitySlots_SlotId",
                table: "PatientAppointments");

            migrationBuilder.DropIndex(
                name: "IX_PatientAppointments_SlotId",
                table: "PatientAppointments");

            migrationBuilder.DropIndex(
                name: "IX_DoctorAvailabilitySlots_DoctorId",
                table: "DoctorAvailabilitySlots");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "DoctorAvailabilitySlots");

            migrationBuilder.DropColumn(
                name: "DoctorProfileId",
                table: "DoctorAvailabilitySlots");

            migrationBuilder.DropColumn(
                name: "IsOpenByDoctor",
                table: "DoctorAvailabilitySlots");

            migrationBuilder.RenameColumn(
                name: "SlotDateTime",
                table: "DoctorAvailabilitySlots",
                newName: "SlotDateTimeUtc");

            migrationBuilder.AddColumn<int>(
                name: "ConsultationDurationMinutes",
                table: "DoctorProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ConsultationPrice",
                table: "DoctorProfiles",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "WorkingEndTime",
                table: "DoctorProfiles",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "WorkingStartTime",
                table: "DoctorProfiles",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "DoctorAvailabilitySlots",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_PatientAppointments_SlotId",
                table: "PatientAppointments",
                column: "SlotId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DoctorAvailabilitySlots_DoctorId_SlotDateTimeUtc",
                table: "DoctorAvailabilitySlots",
                columns: new[] { "DoctorId", "SlotDateTimeUtc" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAppointments_DoctorAvailabilitySlots_SlotId",
                table: "PatientAppointments",
                column: "SlotId",
                principalTable: "DoctorAvailabilitySlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientAppointments_DoctorAvailabilitySlots_SlotId",
                table: "PatientAppointments");

            migrationBuilder.DropIndex(
                name: "IX_PatientAppointments_SlotId",
                table: "PatientAppointments");

            migrationBuilder.DropIndex(
                name: "IX_DoctorAvailabilitySlots_DoctorId_SlotDateTimeUtc",
                table: "DoctorAvailabilitySlots");

            migrationBuilder.DropColumn(
                name: "ConsultationDurationMinutes",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "ConsultationPrice",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "WorkingEndTime",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "WorkingStartTime",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "DoctorAvailabilitySlots");

            migrationBuilder.RenameColumn(
                name: "SlotDateTimeUtc",
                table: "DoctorAvailabilitySlots",
                newName: "SlotDateTime");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "DoctorAvailabilitySlots",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DoctorProfileId",
                table: "DoctorAvailabilitySlots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsOpenByDoctor",
                table: "DoctorAvailabilitySlots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_PatientAppointments_SlotId",
                table: "PatientAppointments",
                column: "SlotId");

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
    }
}
