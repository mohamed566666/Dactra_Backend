using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class addIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
     

            migrationBuilder.AddColumn<bool>(
                name: "isDelelted",
                table: "Questions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isDelelted",
                table: "Posts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isDelelted",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

       
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropColumn(
                name: "isDelelted",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "isDelelted",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "isDelelted",
                table: "AspNetUsers");

       
        }
    }
}
