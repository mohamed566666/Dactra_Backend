using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class editAllergiesChoronicdiseasesmodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "Allergies",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ChronicDisease",
                table: "Patients");

            migrationBuilder.CreateTable(
                name: "Allergies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allergies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChronicDiseases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChronicDiseases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AllergyPatientProfile",
                columns: table => new
                {
                    AllergiesId = table.Column<int>(type: "int", nullable: false),
                    PatientsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllergyPatientProfile", x => new { x.AllergiesId, x.PatientsId });
                    table.ForeignKey(
                        name: "FK_AllergyPatientProfile_Allergies_AllergiesId",
                        column: x => x.AllergiesId,
                        principalTable: "Allergies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AllergyPatientProfile_Patients_PatientsId",
                        column: x => x.PatientsId,
                        principalTable: "Patients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChronicDiseasePatientProfile",
                columns: table => new
                {
                    ChronicDiseasesId = table.Column<int>(type: "int", nullable: false),
                    PatientsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChronicDiseasePatientProfile", x => new { x.ChronicDiseasesId, x.PatientsId });
                    table.ForeignKey(
                        name: "FK_ChronicDiseasePatientProfile_ChronicDiseases_ChronicDiseasesId",
                        column: x => x.ChronicDiseasesId,
                        principalTable: "ChronicDiseases",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ChronicDiseasePatientProfile_Patients_PatientsId",
                        column: x => x.PatientsId,
                        principalTable: "Patients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllergyPatientProfile_PatientsId",
                table: "AllergyPatientProfile",
                column: "PatientsId");

            migrationBuilder.CreateIndex(
                name: "IX_ChronicDiseasePatientProfile_PatientsId",
                table: "ChronicDiseasePatientProfile",
                column: "PatientsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllergyPatientProfile");

            migrationBuilder.DropTable(
                name: "ChronicDiseasePatientProfile");

            migrationBuilder.DropTable(
                name: "Allergies");

            migrationBuilder.DropTable(
                name: "ChronicDiseases");


            migrationBuilder.AddColumn<string>(
                name: "Allergies",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ChronicDisease",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

        }
    }
}
