using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class RROLS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "04175a4e-0ba8-4029-a058-143074c2f737");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a7eb6df9-6323-49f1-bd57-bb7ca0e285ad");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ac18d5e9-53ce-4e26-a2a4-3230f1292572");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f1bc46f9-f4df-470f-afa0-ed3760cfc0be");

            migrationBuilder.Sql("DELETE FROM AspNetRoles WHERE NormalizedName IN ('ADMIN','DOCTORPROFILE','PATIENTPROFILE','MEDICALTESTPROVIDERPROFILE')");

            // إضافة الرولز الجديدة
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
            { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "Admin", "ADMIN" },
            { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "DoctorProfile", "DOCTORPROFILE" },
            { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "PatientProfile", "PATIENTPROFILE" },
            { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "MedicalTestProviderProfile", "MEDICALTESTPROVIDERPROFILE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "46e0c85f-9306-47ad-8b8f-8540f71d30d9");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7c2ff838-1bfb-44f9-953d-065bf52a42af");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "90a02609-04a3-4b38-b3f6-78d17d95bf52");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c8b14b46-566a-41b9-943b-2e5b7b41e6b2");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "04175a4e-0ba8-4029-a058-143074c2f737", null, "MedicalTestProviderProfile", "MEDICALTESTPROVIDERPROFILE" },
                    { "a7eb6df9-6323-49f1-bd57-bb7ca0e285ad", null, "PatientProfile", "PATIENTPROFILE" },
                    { "ac18d5e9-53ce-4e26-a2a4-3230f1292572", null, "DoctorProfile", "DOCTORPROFILE" },
                    { "f1bc46f9-f4df-470f-afa0-ed3760cfc0be", null, "Admin", "ADMIN" }
                });
        }
    }
}
