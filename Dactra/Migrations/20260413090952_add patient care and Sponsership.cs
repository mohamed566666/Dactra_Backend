using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class addpatientcareandSponsership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DoctorMedicalTestSponsors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    MedicalTestProviderId = table.Column<int>(type: "int", nullable: false),
                    ProviderType = table.Column<int>(type: "int", nullable: false),
                    OfferContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespondedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ParentOfferId = table.Column<int>(type: "int", nullable: true),
                    IsCounterOffer = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorMedicalTestSponsors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorMedicalTestSponsors_DoctorMedicalTestSponsors_ParentOfferId",
                        column: x => x.ParentOfferId,
                        principalTable: "DoctorMedicalTestSponsors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoctorMedicalTestSponsors_DoctorProfiles_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "DoctorProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DoctorMedicalTestSponsors_MedicalTestProviderProfiles_MedicalTestProviderId",
                        column: x => x.MedicalTestProviderId,
                        principalTable: "MedicalTestProviderProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientDoctorCares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientDoctorCares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientDoctorCares_DoctorProfiles_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "DoctorProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientDoctorCares_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorMedicalTestSponsors_DoctorId_ProviderType",
                table: "DoctorMedicalTestSponsors",
                columns: new[] { "DoctorId", "ProviderType" });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorMedicalTestSponsors_MedicalTestProviderId",
                table: "DoctorMedicalTestSponsors",
                column: "MedicalTestProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorMedicalTestSponsors_ParentOfferId",
                table: "DoctorMedicalTestSponsors",
                column: "ParentOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDoctorCares_DoctorId",
                table: "PatientDoctorCares",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDoctorCares_PatientId_DoctorId",
                table: "PatientDoctorCares",
                columns: new[] { "PatientId", "DoctorId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorMedicalTestSponsors");

            migrationBuilder.DropTable(
                name: "PatientDoctorCares");
        }
    }
}
