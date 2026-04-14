using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class uodatemodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.CreateTable(
                name: "PatientReferrals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    SponsorshipId = table.Column<int>(type: "int", nullable: false),
                    ReferredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReceivedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientReferrals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientReferrals_DoctorMedicalTestSponsors_SponsorshipId",
                        column: x => x.SponsorshipId,
                        principalTable: "DoctorMedicalTestSponsors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientReferrals_DoctorProfiles_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "DoctorProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientReferrals_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PatientReferralItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientReferralId = table.Column<int>(type: "int", nullable: false),
                    ProviderOfferingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientReferralItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientReferralItems_PatientReferrals_PatientReferralId",
                        column: x => x.PatientReferralId,
                        principalTable: "PatientReferrals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientReferralItems_ProviderOfferings_ProviderOfferingId",
                        column: x => x.ProviderOfferingId,
                        principalTable: "ProviderOfferings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            

            migrationBuilder.CreateIndex(
                name: "IX_PatientReferralItems_PatientReferralId_ProviderOfferingId",
                table: "PatientReferralItems",
                columns: new[] { "PatientReferralId", "ProviderOfferingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientReferralItems_ProviderOfferingId",
                table: "PatientReferralItems",
                column: "ProviderOfferingId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientReferrals_DoctorId",
                table: "PatientReferrals",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientReferrals_PatientId_SponsorshipId",
                table: "PatientReferrals",
                columns: new[] { "PatientId", "SponsorshipId" });

            migrationBuilder.CreateIndex(
                name: "IX_PatientReferrals_SponsorshipId",
                table: "PatientReferrals",
                column: "SponsorshipId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientReferralItems");

            migrationBuilder.DropTable(
                name: "PatientReferrals");

            
        }
    }
}
