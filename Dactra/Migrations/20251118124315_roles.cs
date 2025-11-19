using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1388a1bc-d025-4204-98ca-33fc757d0dbd");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9f59f8d7-ca22-46f4-afc0-b88f7c948801");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cce6f218-b8b9-473d-b96e-28311edbade2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cf4d8f22-8112-4a52-ad7f-a7c6f9d8ebbf");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "12890007-9ace-4efb-a79d-d876e572d8f3", null, "MedicalTestProvider", "MEDICALTESTPROVIDER" },
                    { "1ba93eb6-5891-4ad6-9ccd-87a4f22d406a", null, "Doctor", "DOCTOR" },
                    { "85927874-9e75-49be-afb0-2a1d3f024c01", null, "Patient", "PATIENT" },
                    { "8850c452-1b5e-4901-9a5b-8b8244cca0f8", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "12890007-9ace-4efb-a79d-d876e572d8f3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1ba93eb6-5891-4ad6-9ccd-87a4f22d406a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "85927874-9e75-49be-afb0-2a1d3f024c01");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8850c452-1b5e-4901-9a5b-8b8244cca0f8");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1388a1bc-d025-4204-98ca-33fc757d0dbd", null, "Doctor", "DOCTOR" },
                    { "9f59f8d7-ca22-46f4-afc0-b88f7c948801", null, "MedicalTestProvider", "MEDICALTESTPROVIDER" },
                    { "cce6f218-b8b9-473d-b96e-28311edbade2", null, "Patient", "PATIENT" },
                    { "cf4d8f22-8112-4a52-ad7f-a7c6f9d8ebbf", null, "Admin", "ADMIN" }
                });
        }
    }
}
