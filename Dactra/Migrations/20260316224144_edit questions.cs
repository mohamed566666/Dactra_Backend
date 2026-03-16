using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class editquestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Majors_MajorId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_MajorId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "MajorId",
                table: "Questions");

            migrationBuilder.AddColumn<int>(
                name: "MajorsId",
                table: "Questions",
                type: "int",
                nullable: true);


            migrationBuilder.CreateIndex(
                name: "IX_Questions_MajorsId",
                table: "Questions",
                column: "MajorsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Majors_MajorsId",
                table: "Questions",
                column: "MajorsId",
                principalTable: "Majors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Majors_MajorsId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_MajorsId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "MajorsId",
                table: "Questions");

            migrationBuilder.AddColumn<int>(
                name: "MajorId",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_MajorId",
                table: "Questions",
                column: "MajorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Majors_MajorId",
                table: "Questions",
                column: "MajorId",
                principalTable: "Majors",
                principalColumn: "Id");
        }
    }
}
