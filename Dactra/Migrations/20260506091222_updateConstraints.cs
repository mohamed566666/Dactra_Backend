using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class updateConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_PatientAppointments_AppointmentId",
                table: "Prescriptions");

            

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_PatientAppointments_AppointmentId",
                table: "Prescriptions",
                column: "AppointmentId",
                principalTable: "PatientAppointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_PatientAppointments_AppointmentId",
                table: "Prescriptions");


            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_PatientAppointments_AppointmentId",
                table: "Prescriptions",
                column: "AppointmentId",
                principalTable: "PatientAppointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
