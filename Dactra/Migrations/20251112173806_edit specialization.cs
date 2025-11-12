using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class editspecialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorProfiles_Majors_specializationId",
                table: "DoctorProfiles");

           

            migrationBuilder.RenameColumn(
                name: "specializationId",
                table: "DoctorProfiles",
                newName: "SpecializationId");

            migrationBuilder.RenameIndex(
                name: "IX_DoctorProfiles_specializationId",
                table: "DoctorProfiles",
                newName: "IX_DoctorProfiles_SpecializationId");

            

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorProfiles_Majors_SpecializationId",
                table: "DoctorProfiles",
                column: "SpecializationId",
                principalTable: "Majors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorProfiles_Majors_SpecializationId",
                table: "DoctorProfiles");

            

            migrationBuilder.RenameColumn(
                name: "SpecializationId",
                table: "DoctorProfiles",
                newName: "specializationId");

            migrationBuilder.RenameIndex(
                name: "IX_DoctorProfiles_SpecializationId",
                table: "DoctorProfiles",
                newName: "IX_DoctorProfiles_specializationId");

            

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorProfiles_Majors_specializationId",
                table: "DoctorProfiles",
                column: "specializationId",
                principalTable: "Majors",
                principalColumn: "Id");
        }
    }
}
