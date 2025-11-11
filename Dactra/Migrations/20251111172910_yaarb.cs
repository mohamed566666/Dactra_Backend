using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class yaarb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'ADMIN')
        BEGIN
            INSERT INTO AspNetRoles (Id, Name, NormalizedName)
            VALUES ('1', 'Admin', 'ADMIN')
        END

        IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'DOCTORPROFILE')
        BEGIN
            INSERT INTO AspNetRoles (Id, Name, NormalizedName)
            VALUES ('2', 'DoctorProfile', 'DOCTORPROFILE')
        END

        IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'PATIENTPROFILE')
        BEGIN
            INSERT INTO AspNetRoles (Id, Name, NormalizedName)
            VALUES ('3', 'PatientProfile', 'PATIENTPROFILE')
        END

        IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE NormalizedName = 'MEDICALTESTPROVIDERPROFILE')
        BEGIN
            INSERT INTO AspNetRoles (Id, Name, NormalizedName)
            VALUES ('4', 'MedicalTestProviderProfile', 'MEDICALTESTPROVIDERPROFILE')
        END
    ");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4bab1927-a1e4-498a-a347-a0717cb28f3d", null, "Admin", "ADMIN" },
                    { "721643b7-6d16-4a71-bd7a-2b585f1d26e4", null, "MedicalTestProviderProfile", "MEDICALTESTPROVIDERPROFILE" },
                    { "c16617a7-c0d6-4298-a80f-354ced5802cf", null, "PatientProfile", "PATIENTPROFILE" },
                    { "ecbcefb0-4ace-4299-b27a-fa0869e9409b", null, "DoctorProfile", "DOCTORPROFILE" }
                });
        }
    }
}
