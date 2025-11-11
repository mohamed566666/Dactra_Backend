using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dactra.Migrations
{
    public partial class addEmailVCerficationClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        IF NOT EXISTS (SELECT 1 FROM [AspNetRoles] WHERE [NormalizedName] = 'ADMIN')
        BEGIN
            INSERT INTO [AspNetRoles] (Id, Name, NormalizedName)
            VALUES (NEWID(), 'Admin', 'ADMIN')
        END

        IF NOT EXISTS (SELECT 1 FROM [AspNetRoles] WHERE [NormalizedName] = 'PATIENTPROFILE')
        BEGIN
            INSERT INTO [AspNetRoles] (Id, Name, NormalizedName)
            VALUES (NEWID(), 'PatientProfile', 'PATIENTPROFILE')
        END

        IF NOT EXISTS (SELECT 1 FROM [AspNetRoles] WHERE [NormalizedName] = 'DOCTORPROFILE')
        BEGIN
            INSERT INTO [AspNetRoles] (Id, Name, NormalizedName)
            VALUES (NEWID(), 'DoctorProfile', 'DOCTORPROFILE')
        END

        IF NOT EXISTS (SELECT 1 FROM [AspNetRoles] WHERE [NormalizedName] = 'MEDICALTESTPROVIDERPROFILE')
        BEGIN
            INSERT INTO [AspNetRoles] (Id, Name, NormalizedName)
            VALUES (NEWID(), 'MedicalTestProviderProfile', 'MEDICALTESTPROVIDERPROFILE')
        END
    ");
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        DELETE FROM [AspNetRoles]
        WHERE [NormalizedName] IN ('ADMIN','PATIENTPROFILE','DOCTORPROFILE','MEDICALTESTPROVIDERPROFILE')
    ");
        }

    }
}
