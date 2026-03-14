using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class addisRefunded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2bc20312-9e54-4630-b9db-610ba278af9a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bd5a3385-0cb5-4bdf-9360-137138723d99");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d2598338-37e3-4686-8935-8bb116972222");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dc601504-fa9e-46e4-8cdc-064332fb0762");

            migrationBuilder.AddColumn<bool>(
                name: "isRefunded",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1fb9e6ae-5194-4b13-a4e7-f743b5fdd2be");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "22062af6-5566-47ca-8fd6-35eb7f499190");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "89c4059d-5987-4340-9fd4-9226abe4350c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fdd4e5ad-cdd5-4b40-9d04-ee3debdedb26");

            migrationBuilder.DropColumn(
                name: "isRefunded",
                table: "Payments");

        }
    }
}
