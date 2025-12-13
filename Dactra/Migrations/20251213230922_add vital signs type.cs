using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class addvitalsignstype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "VitalSigns");

            migrationBuilder.AddColumn<DateTime>(
                name: "RecordedAt",
                table: "VitalSigns",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Value2",
                table: "VitalSigns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VitalSignTypeId",
                table: "VitalSigns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "VitalSignTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsComposite = table.Column<bool>(type: "bit", nullable: false),
                    CompositeFields = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VitalSignTypes", x => x.Id);
                });
            migrationBuilder.CreateIndex(
                name: "IX_VitalSigns_VitalSignTypeId",
                table: "VitalSigns",
                column: "VitalSignTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_VitalSigns_VitalSignTypes_VitalSignTypeId",
                table: "VitalSigns",
                column: "VitalSignTypeId",
                principalTable: "VitalSignTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VitalSigns_VitalSignTypes_VitalSignTypeId",
                table: "VitalSigns");

            migrationBuilder.DropTable(
                name: "VitalSignTypes");

            migrationBuilder.DropIndex(
                name: "IX_VitalSigns_VitalSignTypeId",
                table: "VitalSigns");

            migrationBuilder.DropColumn(
                name: "RecordedAt",
                table: "VitalSigns");

            migrationBuilder.DropColumn(
                name: "Value2",
                table: "VitalSigns");

            migrationBuilder.DropColumn(
                name: "VitalSignTypeId",
                table: "VitalSigns");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "VitalSigns",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
