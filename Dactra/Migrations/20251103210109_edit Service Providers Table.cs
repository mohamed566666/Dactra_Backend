using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class editServiceProvidersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_Providers_DoctorId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Providers_DoctorId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderOfferings_Providers_ProviderId",
                table: "ProviderOfferings");

            migrationBuilder.DropForeignKey(
                name: "FK_Providers_AspNetUsers_UserId",
                table: "Providers");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Providers_ProviderId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleTables_Providers_DoctorId",
                table: "ScheduleTables");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Providers",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "StartingCareerDate",
                table: "Providers");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Providers");

            migrationBuilder.RenameTable(
                name: "Providers",
                newName: "ServiceProviders");

            migrationBuilder.RenameIndex(
                name: "IX_Providers_UserId",
                table: "ServiceProviders",
                newName: "IX_ServiceProviders_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceProviders",
                table: "ServiceProviders",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DoctorProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartingCareerDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorProfiles_ServiceProviders_Id",
                        column: x => x.Id,
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicalTestProviderProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalTestProviderProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalTestProviderProfiles_ServiceProviders_Id",
                        column: x => x.Id,
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_DoctorProfiles_DoctorId",
                table: "Answers",
                column: "DoctorId",
                principalTable: "DoctorProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_DoctorProfiles_DoctorId",
                table: "Posts",
                column: "DoctorId",
                principalTable: "DoctorProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderOfferings_MedicalTestProviderProfiles_ProviderId",
                table: "ProviderOfferings",
                column: "ProviderId",
                principalTable: "MedicalTestProviderProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_ServiceProviders_ProviderId",
                table: "Ratings",
                column: "ProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleTables_DoctorProfiles_DoctorId",
                table: "ScheduleTables",
                column: "DoctorId",
                principalTable: "DoctorProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviders_AspNetUsers_UserId",
                table: "ServiceProviders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answers_DoctorProfiles_DoctorId",
                table: "Answers");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_DoctorProfiles_DoctorId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderOfferings_MedicalTestProviderProfiles_ProviderId",
                table: "ProviderOfferings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_ServiceProviders_ProviderId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleTables_DoctorProfiles_DoctorId",
                table: "ScheduleTables");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviders_AspNetUsers_UserId",
                table: "ServiceProviders");

            migrationBuilder.DropTable(
                name: "DoctorProfiles");

            migrationBuilder.DropTable(
                name: "MedicalTestProviderProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceProviders",
                table: "ServiceProviders");

            migrationBuilder.RenameTable(
                name: "ServiceProviders",
                newName: "Providers");

            migrationBuilder.RenameIndex(
                name: "IX_ServiceProviders_UserId",
                table: "Providers",
                newName: "IX_Providers_UserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Providers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Providers",
                type: "nvarchar(34)",
                maxLength: 34,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Providers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "Providers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Providers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartingCareerDate",
                table: "Providers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Providers",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Providers",
                table: "Providers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Answers_Providers_DoctorId",
                table: "Answers",
                column: "DoctorId",
                principalTable: "Providers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Providers_DoctorId",
                table: "Posts",
                column: "DoctorId",
                principalTable: "Providers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderOfferings_Providers_ProviderId",
                table: "ProviderOfferings",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Providers_AspNetUsers_UserId",
                table: "Providers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Providers_ProviderId",
                table: "Ratings",
                column: "ProviderId",
                principalTable: "Providers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleTables_Providers_DoctorId",
                table: "ScheduleTables",
                column: "DoctorId",
                principalTable: "Providers",
                principalColumn: "Id");
        }
    }
}
