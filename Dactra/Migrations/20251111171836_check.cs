using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class check : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2624b06e-a796-4bd9-a821-9a5b0e9383d7");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c43e476-9e89-410e-a129-0e6865fc1a3e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "39a8d614-da3b-4b71-9074-d4b47801f4c9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "75bd61c4-ee69-44fa-9e20-6c5411893876");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2624b06e-a796-4bd9-a821-9a5b0e9383d7", null, "MedicalTestProviderProfile", "MEDICALTESTPROVIDERPROFILE" },
                    { "2c43e476-9e89-410e-a129-0e6865fc1a3e", null, "Admin", "ADMIN" },
                    { "39a8d614-da3b-4b71-9074-d4b47801f4c9", null, "PatientProfile", "PATIENTPROFILE" },
                    { "75bd61c4-ee69-44fa-9e20-6c5411893876", null, "DoctorProfile", "DOCTORPROFILE" }
                });
        }
    }
}
