using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GZCTF.Migrations
{
    /// <inheritdoc />
    public partial class AddClassExerciseMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ExerciseChallenges",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClassExercises",
                columns: table => new
                {
                    ClassId = table.Column<int>(type: "integer", nullable: false),
                    ExerciseId = table.Column<int>(type: "integer", nullable: false),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    VisibleFromUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DueUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassExercises", x => new { x.ClassId, x.ExerciseId });
                    table.ForeignKey(
                        name: "FK_ClassExercises_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassExercises_ExerciseChallenges_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "ExerciseChallenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassExercises_ClassId",
                table: "ClassExercises",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassExercises_ExerciseId",
                table: "ClassExercises",
                column: "ExerciseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassExercises");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ExerciseChallenges");
        }
    }
}
