using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class add_amount_to_scheduale_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
   
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "ScheduleTables",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

        

            migrationBuilder.AlterColumn<int>(
                name: "PrescriptionId",
                table: "PatientAppointments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

        
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
   
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "ScheduleTables");

            migrationBuilder.DropColumn(
                name: "Heading",
                table: "Ratings");

            migrationBuilder.AlterColumn<int>(
                name: "PrescriptionId",
                table: "PatientAppointments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

        
        }
    }
}
