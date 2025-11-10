using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class deletewrongnavigations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorProfiles_ServiceProviders_ProviderId",
                table: "DoctorProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalTestProviderProfiles_ServiceProviders_ProviderId",
                table: "MedicalTestProviderProfiles");

            migrationBuilder.DropIndex(
                name: "IX_MedicalTestProviderProfiles_ProviderId",
                table: "MedicalTestProviderProfiles");

            migrationBuilder.DropIndex(
                name: "IX_DoctorProfiles_ProviderId",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "MedicalTestProviderProfiles");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "DoctorProfiles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProviderId",
                table: "MedicalTestProviderProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProviderId",
                table: "DoctorProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalTestProviderProfiles_ProviderId",
                table: "MedicalTestProviderProfiles",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorProfiles_ProviderId",
                table: "DoctorProfiles",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorProfiles_ServiceProviders_ProviderId",
                table: "DoctorProfiles",
                column: "ProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalTestProviderProfiles_ServiceProviders_ProviderId",
                table: "MedicalTestProviderProfiles",
                column: "ProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id");
        }
    }
}
