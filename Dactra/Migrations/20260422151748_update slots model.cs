using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class updateslotsmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OnlineConsultationDurationMinutes",
                table: "DoctorProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OnlineConsultationPrice",
                table: "DoctorProfiles",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "OnlineWorkingEndTime",
                table: "DoctorProfiles",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "OnlineWorkingStartTime",
                table: "DoctorProfiles",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SlotType",
                table: "DoctorAvailabilitySlots",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnlineConsultationDurationMinutes",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "OnlineConsultationPrice",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "OnlineWorkingEndTime",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "OnlineWorkingStartTime",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "SlotType",
                table: "DoctorAvailabilitySlots");
        }
    }
}
