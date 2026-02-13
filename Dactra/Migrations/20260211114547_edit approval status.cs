using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class editapprovalstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "ServiceProviders");

            migrationBuilder.AddColumn<int>(
                name: "approvalStatus",
                table: "ServiceProviders",
                type: "int",
                nullable: false,
                defaultValue: 0);

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropColumn(
                name: "approvalStatus",
                table: "ServiceProviders");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "ServiceProviders",
                type: "bit",
                nullable: false,
                defaultValue: false);

           
        }
    }
}
