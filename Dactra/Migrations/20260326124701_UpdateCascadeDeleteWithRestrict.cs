using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCascadeDeleteWithRestrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AllergyPatientProfile_Allergies_AllergiesId",
                table: "AllergyPatientProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_AllergyPatientProfile_Patients_PatientsId",
                table: "AllergyPatientProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_ChronicDiseasePatientProfile_ChronicDiseases_ChronicDiseasesId",
                table: "ChronicDiseasePatientProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_ChronicDiseasePatientProfile_Patients_PatientsId",
                table: "ChronicDiseasePatientProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_AspNetUsers_UserId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_Posts_PostId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_AspNetUsers_UserId",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorAvailabilitySlots_DoctorProfiles_DoctorId",
                table: "DoctorAvailabilitySlots");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorProfiles_Majors_SpecializationId",
                table: "DoctorProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorQualifications_DoctorProfiles_DoctorProfileId",
                table: "DoctorQualifications");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicinesPrescription_Medicines_MedicinesId",
                table: "MedicinesPrescription");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicinesPrescription_Prescriptions_prescriptionsId",
                table: "MedicinesPrescription");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientAppointments_DoctorAvailabilitySlots_SlotId",
                table: "PatientAppointments");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientAppointments_Patients_PatientId",
                table: "PatientAppointments");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientAppointments_Payments_PaymentId",
                table: "PatientAppointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_AspNetUsers_UserId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_PostLikes_Posts_PostId",
                table: "PostLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_DoctorProfiles_DoctorId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTags_Posts_PostId",
                table: "PostTags");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTags_Tags_TagId",
                table: "PostTags");

            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionWithMedicin_Medicines_MedicinesId",
                table: "PrescriptionWithMedicin");

            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionWithMedicin_Prescriptions_PrescriptionId",
                table: "PrescriptionWithMedicin");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderOfferings_MedicalTestProviderProfiles_ProviderId",
                table: "ProviderOfferings");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderOfferings_TestServices_TestServiceId",
                table: "ProviderOfferings");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_DoctorProfiles_DoctorId",
                table: "QuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_Questions_QuestionId",
                table: "QuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionInterests_AspNetUsers_UserId",
                table: "QuestionInterests");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionInterests_Questions_QuestionId",
                table: "QuestionInterests");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Patients_PatientId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionSaves_AspNetUsers_UserId",
                table: "QuestionSaves");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionSaves_Questions_QuestionId",
                table: "QuestionSaves");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTags_Questions_QuestionId",
                table: "QuestionTags");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTags_Tags_TagId",
                table: "QuestionTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Patients_PatientId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_ServiceProviders_ProviderId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedPosts_AspNetUsers_UserId",
                table: "SavedPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedPosts_Posts_PostId",
                table: "SavedPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviders_AspNetUsers_UserId",
                table: "ServiceProviders");

            migrationBuilder.DropForeignKey(
                name: "FK_VitalSigns_Patients_PatientId",
                table: "VitalSigns");

            migrationBuilder.DropForeignKey(
                name: "FK_VitalSigns_VitalSignTypes_VitalSignTypeId",
                table: "VitalSigns");

           

            migrationBuilder.AddColumn<int>(
                name: "ServiceProviderProfileId",
                table: "Ratings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentId1",
                table: "PatientAppointments",
                type: "int",
                nullable: true);

           

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_ServiceProviderProfileId",
                table: "Ratings",
                column: "ServiceProviderProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAppointments_PaymentId1",
                table: "PatientAppointments",
                column: "PaymentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AllergyPatientProfile_Allergies_AllergiesId",
                table: "AllergyPatientProfile",
                column: "AllergiesId",
                principalTable: "Allergies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AllergyPatientProfile_Patients_PatientsId",
                table: "AllergyPatientProfile",
                column: "PatientsId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChronicDiseasePatientProfile_ChronicDiseases_ChronicDiseasesId",
                table: "ChronicDiseasePatientProfile",
                column: "ChronicDiseasesId",
                principalTable: "ChronicDiseases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChronicDiseasePatientProfile_Patients_PatientsId",
                table: "ChronicDiseasePatientProfile",
                column: "PatientsId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_AspNetUsers_UserId",
                table: "comments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_Posts_PostId",
                table: "comments",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_AspNetUsers_UserId",
                table: "Complaints",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorAvailabilitySlots_DoctorProfiles_DoctorId",
                table: "DoctorAvailabilitySlots",
                column: "DoctorId",
                principalTable: "DoctorProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorProfiles_Majors_SpecializationId",
                table: "DoctorProfiles",
                column: "SpecializationId",
                principalTable: "Majors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorQualifications_DoctorProfiles_DoctorProfileId",
                table: "DoctorQualifications",
                column: "DoctorProfileId",
                principalTable: "DoctorProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicinesPrescription_Medicines_MedicinesId",
                table: "MedicinesPrescription",
                column: "MedicinesId",
                principalTable: "Medicines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicinesPrescription_Prescriptions_prescriptionsId",
                table: "MedicinesPrescription",
                column: "prescriptionsId",
                principalTable: "Prescriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAppointments_DoctorAvailabilitySlots_SlotId",
                table: "PatientAppointments",
                column: "SlotId",
                principalTable: "DoctorAvailabilitySlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAppointments_Patients_PatientId",
                table: "PatientAppointments",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAppointments_Payments_PaymentId",
                table: "PatientAppointments",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAppointments_Payments_PaymentId1",
                table: "PatientAppointments",
                column: "PaymentId1",
                principalTable: "Payments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_AspNetUsers_UserId",
                table: "Patients",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostLikes_Posts_PostId",
                table: "PostLikes",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_DoctorProfiles_DoctorId",
                table: "Posts",
                column: "DoctorId",
                principalTable: "DoctorProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTags_Posts_PostId",
                table: "PostTags",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTags_Tags_TagId",
                table: "PostTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionWithMedicin_Medicines_MedicinesId",
                table: "PrescriptionWithMedicin",
                column: "MedicinesId",
                principalTable: "Medicines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionWithMedicin_Prescriptions_PrescriptionId",
                table: "PrescriptionWithMedicin",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderOfferings_MedicalTestProviderProfiles_ProviderId",
                table: "ProviderOfferings",
                column: "ProviderId",
                principalTable: "MedicalTestProviderProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderOfferings_TestServices_TestServiceId",
                table: "ProviderOfferings",
                column: "TestServiceId",
                principalTable: "TestServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_DoctorProfiles_DoctorId",
                table: "QuestionAnswers",
                column: "DoctorId",
                principalTable: "DoctorProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_Questions_QuestionId",
                table: "QuestionAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionInterests_AspNetUsers_UserId",
                table: "QuestionInterests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionInterests_Questions_QuestionId",
                table: "QuestionInterests",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Patients_PatientId",
                table: "Questions",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionSaves_AspNetUsers_UserId",
                table: "QuestionSaves",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionSaves_Questions_QuestionId",
                table: "QuestionSaves",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTags_Questions_QuestionId",
                table: "QuestionTags",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTags_Tags_TagId",
                table: "QuestionTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Patients_PatientId",
                table: "Ratings",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_ServiceProviders_ProviderId",
                table: "Ratings",
                column: "ProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_ServiceProviders_ServiceProviderProfileId",
                table: "Ratings",
                column: "ServiceProviderProfileId",
                principalTable: "ServiceProviders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedPosts_AspNetUsers_UserId",
                table: "SavedPosts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedPosts_Posts_PostId",
                table: "SavedPosts",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviders_AspNetUsers_UserId",
                table: "ServiceProviders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VitalSigns_Patients_PatientId",
                table: "VitalSigns",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VitalSigns_VitalSignTypes_VitalSignTypeId",
                table: "VitalSigns",
                column: "VitalSignTypeId",
                principalTable: "VitalSignTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AllergyPatientProfile_Allergies_AllergiesId",
                table: "AllergyPatientProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_AllergyPatientProfile_Patients_PatientsId",
                table: "AllergyPatientProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_ChronicDiseasePatientProfile_ChronicDiseases_ChronicDiseasesId",
                table: "ChronicDiseasePatientProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_ChronicDiseasePatientProfile_Patients_PatientsId",
                table: "ChronicDiseasePatientProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_AspNetUsers_UserId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_Posts_PostId",
                table: "comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_AspNetUsers_UserId",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorAvailabilitySlots_DoctorProfiles_DoctorId",
                table: "DoctorAvailabilitySlots");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorProfiles_Majors_SpecializationId",
                table: "DoctorProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorQualifications_DoctorProfiles_DoctorProfileId",
                table: "DoctorQualifications");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicinesPrescription_Medicines_MedicinesId",
                table: "MedicinesPrescription");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicinesPrescription_Prescriptions_prescriptionsId",
                table: "MedicinesPrescription");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientAppointments_DoctorAvailabilitySlots_SlotId",
                table: "PatientAppointments");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientAppointments_Patients_PatientId",
                table: "PatientAppointments");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientAppointments_Payments_PaymentId",
                table: "PatientAppointments");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientAppointments_Payments_PaymentId1",
                table: "PatientAppointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_AspNetUsers_UserId",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_PostLikes_Posts_PostId",
                table: "PostLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_DoctorProfiles_DoctorId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTags_Posts_PostId",
                table: "PostTags");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTags_Tags_TagId",
                table: "PostTags");

            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionWithMedicin_Medicines_MedicinesId",
                table: "PrescriptionWithMedicin");

            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionWithMedicin_Prescriptions_PrescriptionId",
                table: "PrescriptionWithMedicin");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderOfferings_MedicalTestProviderProfiles_ProviderId",
                table: "ProviderOfferings");

            migrationBuilder.DropForeignKey(
                name: "FK_ProviderOfferings_TestServices_TestServiceId",
                table: "ProviderOfferings");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_DoctorProfiles_DoctorId",
                table: "QuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_Questions_QuestionId",
                table: "QuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionInterests_AspNetUsers_UserId",
                table: "QuestionInterests");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionInterests_Questions_QuestionId",
                table: "QuestionInterests");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Patients_PatientId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionSaves_AspNetUsers_UserId",
                table: "QuestionSaves");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionSaves_Questions_QuestionId",
                table: "QuestionSaves");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTags_Questions_QuestionId",
                table: "QuestionTags");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionTags_Tags_TagId",
                table: "QuestionTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Patients_PatientId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_ServiceProviders_ProviderId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_ServiceProviders_ServiceProviderProfileId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedPosts_AspNetUsers_UserId",
                table: "SavedPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedPosts_Posts_PostId",
                table: "SavedPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceProviders_AspNetUsers_UserId",
                table: "ServiceProviders");

            migrationBuilder.DropForeignKey(
                name: "FK_VitalSigns_Patients_PatientId",
                table: "VitalSigns");

            migrationBuilder.DropForeignKey(
                name: "FK_VitalSigns_VitalSignTypes_VitalSignTypeId",
                table: "VitalSigns");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_ServiceProviderProfileId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_PatientAppointments_PaymentId1",
                table: "PatientAppointments");

            

            migrationBuilder.DropColumn(
                name: "ServiceProviderProfileId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "PaymentId1",
                table: "PatientAppointments");

            

            migrationBuilder.AddForeignKey(
                name: "FK_AllergyPatientProfile_Allergies_AllergiesId",
                table: "AllergyPatientProfile",
                column: "AllergiesId",
                principalTable: "Allergies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AllergyPatientProfile_Patients_PatientsId",
                table: "AllergyPatientProfile",
                column: "PatientsId",
                principalTable: "Patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChronicDiseasePatientProfile_ChronicDiseases_ChronicDiseasesId",
                table: "ChronicDiseasePatientProfile",
                column: "ChronicDiseasesId",
                principalTable: "ChronicDiseases",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChronicDiseasePatientProfile_Patients_PatientsId",
                table: "ChronicDiseasePatientProfile",
                column: "PatientsId",
                principalTable: "Patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_AspNetUsers_UserId",
                table: "comments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_comments_Posts_PostId",
                table: "comments",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_AspNetUsers_UserId",
                table: "Complaints",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorAvailabilitySlots_DoctorProfiles_DoctorId",
                table: "DoctorAvailabilitySlots",
                column: "DoctorId",
                principalTable: "DoctorProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorProfiles_Majors_SpecializationId",
                table: "DoctorProfiles",
                column: "SpecializationId",
                principalTable: "Majors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorQualifications_DoctorProfiles_DoctorProfileId",
                table: "DoctorQualifications",
                column: "DoctorProfileId",
                principalTable: "DoctorProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicinesPrescription_Medicines_MedicinesId",
                table: "MedicinesPrescription",
                column: "MedicinesId",
                principalTable: "Medicines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicinesPrescription_Prescriptions_prescriptionsId",
                table: "MedicinesPrescription",
                column: "prescriptionsId",
                principalTable: "Prescriptions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAppointments_DoctorAvailabilitySlots_SlotId",
                table: "PatientAppointments",
                column: "SlotId",
                principalTable: "DoctorAvailabilitySlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAppointments_Patients_PatientId",
                table: "PatientAppointments",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAppointments_Payments_PaymentId",
                table: "PatientAppointments",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_AspNetUsers_UserId",
                table: "Patients",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostLikes_Posts_PostId",
                table: "PostLikes",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_DoctorProfiles_DoctorId",
                table: "Posts",
                column: "DoctorId",
                principalTable: "DoctorProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostTags_Posts_PostId",
                table: "PostTags",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostTags_Tags_TagId",
                table: "PostTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionWithMedicin_Medicines_MedicinesId",
                table: "PrescriptionWithMedicin",
                column: "MedicinesId",
                principalTable: "Medicines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionWithMedicin_Prescriptions_PrescriptionId",
                table: "PrescriptionWithMedicin",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderOfferings_MedicalTestProviderProfiles_ProviderId",
                table: "ProviderOfferings",
                column: "ProviderId",
                principalTable: "MedicalTestProviderProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProviderOfferings_TestServices_TestServiceId",
                table: "ProviderOfferings",
                column: "TestServiceId",
                principalTable: "TestServices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_DoctorProfiles_DoctorId",
                table: "QuestionAnswers",
                column: "DoctorId",
                principalTable: "DoctorProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_Questions_QuestionId",
                table: "QuestionAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionInterests_AspNetUsers_UserId",
                table: "QuestionInterests",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionInterests_Questions_QuestionId",
                table: "QuestionInterests",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Patients_PatientId",
                table: "Questions",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionSaves_AspNetUsers_UserId",
                table: "QuestionSaves",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionSaves_Questions_QuestionId",
                table: "QuestionSaves",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTags_Questions_QuestionId",
                table: "QuestionTags",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionTags_Tags_TagId",
                table: "QuestionTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Patients_PatientId",
                table: "Ratings",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_ServiceProviders_ProviderId",
                table: "Ratings",
                column: "ProviderId",
                principalTable: "ServiceProviders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedPosts_AspNetUsers_UserId",
                table: "SavedPosts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SavedPosts_Posts_PostId",
                table: "SavedPosts",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceProviders_AspNetUsers_UserId",
                table: "ServiceProviders",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VitalSigns_Patients_PatientId",
                table: "VitalSigns",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VitalSigns_VitalSignTypes_VitalSignTypeId",
                table: "VitalSigns",
                column: "VitalSignTypeId",
                principalTable: "VitalSignTypes",
                principalColumn: "Id");
        }
    }
}
