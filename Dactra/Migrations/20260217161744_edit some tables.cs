using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class editsometables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "009e6c4b-dd66-46a4-bc1b-da2fba473a1e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "386530a6-1a8f-4f2d-beec-840bff4f86c0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6443c647-66b7-4d5f-ad43-ff0c951a48a4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c203944a-4869-4718-8bf6-b1eb16e08612");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Payments",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "Payments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsHmacVerified",
                table: "Payments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PaymobOrderId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymobPaymentKey",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymobTransactionId",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReserved",
                table: "DoctorAvailabilitySlots",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReservedUntil",
                table: "DoctorAvailabilitySlots",
                type: "datetime2",
                nullable: true);

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsHmacVerified",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymobOrderId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymobPaymentKey",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymobTransactionId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "IsReserved",
                table: "DoctorAvailabilitySlots");

            migrationBuilder.DropColumn(
                name: "ReservedUntil",
                table: "DoctorAvailabilitySlots");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Payments",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

           
        }
    }
}
