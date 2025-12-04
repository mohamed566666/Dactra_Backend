using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class FixIsDeletedColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.RenameColumn(
                name: "isDelelted",
                table: "Questions",
                newName: "isDeleted");

            migrationBuilder.RenameColumn(
                name: "isDelelted",
                table: "Posts",
                newName: "isDeleted");

            migrationBuilder.RenameColumn(
                name: "isDelelted",
                table: "AspNetUsers",
                newName: "isDeleted");

        
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
     

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "Questions",
                newName: "isDelelted");

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "Posts",
                newName: "isDelelted");

            migrationBuilder.RenameColumn(
                name: "isDeleted",
                table: "AspNetUsers",
                newName: "isDelelted");

          
        }
    }
}
