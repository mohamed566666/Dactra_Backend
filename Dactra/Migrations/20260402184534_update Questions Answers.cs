using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dactra.Migrations
{
    /// <inheritdoc />
    public partial class updateQuestionsAnswers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 🗑️ أولاً: امسح الداتا القديمة عشان نتجنب مشاكل الـ FK
            migrationBuilder.Sql("DELETE FROM [QuestionAnswers]");

            // ── 1. حذف الـ FK والـ Index القديم ──
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_DoctorProfiles_DoctorId",
                table: "QuestionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_QuestionAnswers_DoctorId",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "QuestionAnswers");

            // ── 2. إضافة AnswererUserId ──
            migrationBuilder.AddColumn<string>(
                name: "AnswererUserId",
                table: "QuestionAnswers",
                type: "nvarchar(450)",  // ← نفس طول AspNetUsers.Id
                nullable: false,
                defaultValue: "");

            // ── 3. إضافة الـ Index والـ ForeignKey ──
            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswers_AnswererUserId",
                table: "QuestionAnswers",
                column: "AnswererUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_AspNetUsers_AnswererUserId",
                table: "QuestionAnswers",
                column: "AnswererUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // ── 4. إنشاء جدول الـ QuestionAnswerLikes ──
            migrationBuilder.CreateTable(
                name: "QuestionAnswerLikes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AnswerId = table.Column<int>(type: "int", nullable: false),

                    // ✅ التصحيح المهم هنا: من nvarchar(max) إلى nvarchar(450)
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),

                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionAnswerLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionAnswerLikes_QuestionAnswers_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "QuestionAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswerLikes_AnswerId",
                table: "QuestionAnswerLikes",
                column: "AnswerId");

            // ✅ الآن الـ Unique Index هينجح لأن UserId نوعه nvarchar(450)
            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswerLikes_AnswerId_UserId",
                table: "QuestionAnswerLikes",
                columns: new[] { "AnswerId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QuestionAnswerLikes_AnswerId_UserId",
                table: "QuestionAnswerLikes");

            migrationBuilder.DropIndex(
                name: "IX_QuestionAnswerLikes_AnswerId",
                table: "QuestionAnswerLikes");

            migrationBuilder.DropTable(
                name: "QuestionAnswerLikes");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_AspNetUsers_AnswererUserId",
                table: "QuestionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_QuestionAnswers_AnswererUserId",
                table: "QuestionAnswers");

            migrationBuilder.DropColumn(
                name: "AnswererUserId",
                table: "QuestionAnswers");

            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                table: "QuestionAnswers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswers_DoctorId",
                table: "QuestionAnswers",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_DoctorProfiles_DoctorId",
                table: "QuestionAnswers",
                column: "DoctorId",
                principalTable: "DoctorProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}