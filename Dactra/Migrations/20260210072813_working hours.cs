using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class workinghours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4eeea624-46f4-4c18-b947-d09623ad5415");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5676b94b-88ff-44f0-87ac-0708146d2682");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "896ca909-6932-49ea-8065-6366b533c194");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d062c6ce-0d1e-4571-b232-a283d0953171");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3b74380f-a2f4-406a-b9db-6df92117c395", null, "Patient", "PATIENT" },
                    { "5ad8ef78-5b6f-4f0d-9a29-c02d4d532c78", null, "MedicalTestProvider", "MEDICALTESTPROVIDER" },
                    { "677cfc5d-89f6-4e64-b12f-b82e86b198af", null, "Admin", "ADMIN" },
                    { "9f99a251-236f-404b-ae96-9b70ce1c08ac", null, "Doctor", "DOCTOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3b74380f-a2f4-406a-b9db-6df92117c395");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ad8ef78-5b6f-4f0d-9a29-c02d4d532c78");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "677cfc5d-89f6-4e64-b12f-b82e86b198af");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9f99a251-236f-404b-ae96-9b70ce1c08ac");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4eeea624-46f4-4c18-b947-d09623ad5415", null, "Patient", "PATIENT" },
                    { "5676b94b-88ff-44f0-87ac-0708146d2682", null, "MedicalTestProvider", "MEDICALTESTPROVIDER" },
                    { "896ca909-6932-49ea-8065-6366b533c194", null, "Admin", "ADMIN" },
                    { "d062c6ce-0d1e-4571-b232-a283d0953171", null, "Doctor", "DOCTOR" }
                });
        }
    }
}
