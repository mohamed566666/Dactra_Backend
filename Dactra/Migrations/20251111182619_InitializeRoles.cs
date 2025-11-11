using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class InitializeRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "23796220-f957-44c5-b329-816ff12a8425", null, "Admin", "ADMIN" },
                    { "438786ff-b7e4-4520-bf21-0f4f738293f9", null, "MedicalTestProviderProfile", "MEDICALTESTPROVIDERPROFILE" },
                    { "87e91a56-cf1d-4af9-bd9a-2c16936042b1", null, "PatientProfile", "PATIENTPROFILE" },
                    { "c0edab8f-03f3-46db-b058-a8de9187ee01", null, "DoctorProfile", "DOCTORPROFILE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "23796220-f957-44c5-b329-816ff12a8425");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "438786ff-b7e4-4520-bf21-0f4f738293f9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "87e91a56-cf1d-4af9-bd9a-2c16936042b1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c0edab8f-03f3-46db-b058-a8de9187ee01");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", null, "Admin", "ADMIN" },
                    { "2", null, "DoctorProfile", "DOCTORPROFILE" },
                    { "3", null, "PatientProfile", "PATIENTPROFILE" },
                    { "4", null, "MedicalTestProviderProfile", "MEDICALTESTPROVIDERPROFILE" }
                });
        }
    }
}
