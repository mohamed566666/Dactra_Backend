using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class addspecializationtoDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<int>(
                name: "specializationId",
                table: "DoctorProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DoctorProfiles_specializationId",
                table: "DoctorProfiles",
                column: "specializationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorProfiles_Majors_specializationId",
                table: "DoctorProfiles",
                column: "specializationId",
                principalTable: "Majors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorProfiles_Majors_specializationId",
                table: "DoctorProfiles");

            migrationBuilder.DropIndex(
                name: "IX_DoctorProfiles_specializationId",
                table: "DoctorProfiles");

            

            migrationBuilder.DropColumn(
                name: "specializationId",
                table: "DoctorProfiles");

            
        }
    }
}
