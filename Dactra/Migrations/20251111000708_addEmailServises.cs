using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class addEmailServises : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "EmailVerifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OTP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailVerifications", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "48ded938-de90-4f31-9dbf-6c777045d626", null, "Admin", "ADMIN" },
                    { "744f6d34-4da2-429c-9b65-1e4b7e4c6252", null, "MedicalTestProviderProfile", "MEDICALTESTPROVIDERPROFILE" },
                    { "a0a44a8d-a097-4dc6-86e7-74ab4b7d1b65", null, "PatientProfile", "PATIENTPROFILE" },
                    { "c2e814cd-8e6b-401c-b5fb-50f90062511f", null, "DoctorProfile", "DOCTORPROFILE" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailVerifications");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48ded938-de90-4f31-9dbf-6c777045d626");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "744f6d34-4da2-429c-9b65-1e4b7e4c6252");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a0a44a8d-a097-4dc6-86e7-74ab4b7d1b65");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c2e814cd-8e6b-401c-b5fb-50f90062511f");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "46e0c85f-9306-47ad-8b8f-8540f71d30d9", null, "DoctorProfile", "DOCTORPROFILE" },
                    { "7c2ff838-1bfb-44f9-953d-065bf52a42af", null, "Admin", "ADMIN" },
                    { "90a02609-04a3-4b38-b3f6-78d17d95bf52", null, "MedicalTestProviderProfile", "MEDICALTESTPROVIDERPROFILE" },
                    { "c8b14b46-566a-41b9-943b-2e5b7b41e6b2", null, "PatientProfile", "PATIENTPROFILE" }
                });
        }
    }
}
