using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class trythis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4bab1927-a1e4-498a-a347-a0717cb28f3d", null, "Admin", "ADMIN" },
                    { "721643b7-6d16-4a71-bd7a-2b585f1d26e4", null, "MedicalTestProviderProfile", "MEDICALTESTPROVIDERPROFILE" },
                    { "c16617a7-c0d6-4298-a80f-354ced5802cf", null, "PatientProfile", "PATIENTPROFILE" },
                    { "ecbcefb0-4ace-4299-b27a-fa0869e9409b", null, "DoctorProfile", "DOCTORPROFILE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4bab1927-a1e4-498a-a347-a0717cb28f3d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "721643b7-6d16-4a71-bd7a-2b585f1d26e4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c16617a7-c0d6-4298-a80f-354ced5802cf");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ecbcefb0-4ace-4299-b27a-fa0869e9409b");
        }
    }
}
