using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GZCTF.Migrations
{
    /// <inheritdoc />
    public partial class AddClassesAndLabs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_AspNetUsers_OwnerId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassMembers_AspNetUsers_UserId",
                table: "ClassMembers");

            migrationBuilder.DropIndex(
                name: "IX_ClassMembers_ClassId_RoleInClass",
                table: "ClassMembers");

            migrationBuilder.DropIndex(
                name: "IX_Classes_OwnerId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "RoleInClass",
                table: "ClassMembers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ClassMembers");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Term",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "Classes");

            migrationBuilder.RenameColumn(
                name: "JoinedAtUtc",
                table: "ClassMembers",
                newName: "JoinedUtc");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ClassMembers",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_ClassMembers_UserId",
                table: "ClassMembers",
                newName: "IX_ClassMembers_StudentId");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "Classes",
                newName: "CreatedUtc");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Classes",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(120)",
                oldMaxLength: 120);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Classes",
                type: "character varying(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Classes",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId",
                table: "Classes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "LabSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClassId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    StartUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EndUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabSessions_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SessionId = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<Guid>(type: "uuid", nullable: false),
                    AttachmentId = table.Column<int>(type: "integer", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true),
                    SubmittedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: true),
                    Feedback = table.Column<string>(type: "text", nullable: true),
                    GradedById = table.Column<Guid>(type: "uuid", nullable: true),
                    GradedUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabReports_AspNetUsers_GradedById",
                        column: x => x.GradedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LabReports_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabReports_Attachments_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LabReports_LabSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "LabSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabSessionChallenges",
                columns: table => new
                {
                    SessionId = table.Column<int>(type: "integer", nullable: false),
                    ExerciseId = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<int>(type: "integer", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Required = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabSessionChallenges", x => new { x.SessionId, x.ExerciseId });
                    table.ForeignKey(
                        name: "FK_LabSessionChallenges_ExerciseChallenges_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "ExerciseChallenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabSessionChallenges_LabSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "LabSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Classes_TeacherId",
                table: "Classes",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_LabReports_AttachmentId",
                table: "LabReports",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LabReports_GradedById",
                table: "LabReports",
                column: "GradedById");

            migrationBuilder.CreateIndex(
                name: "IX_LabReports_SessionId",
                table: "LabReports",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_LabReports_StudentId",
                table: "LabReports",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_LabReports_StudentId_SessionId",
                table: "LabReports",
                columns: new[] { "StudentId", "SessionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabSessionChallenges_ExerciseId",
                table: "LabSessionChallenges",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_LabSessions_ClassId",
                table: "LabSessions",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_AspNetUsers_TeacherId",
                table: "Classes",
                column: "TeacherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassMembers_AspNetUsers_StudentId",
                table: "ClassMembers",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_AspNetUsers_TeacherId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassMembers_AspNetUsers_StudentId",
                table: "ClassMembers");

            migrationBuilder.DropTable(
                name: "LabReports");

            migrationBuilder.DropTable(
                name: "LabSessionChallenges");

            migrationBuilder.DropTable(
                name: "LabSessions");

            migrationBuilder.DropIndex(
                name: "IX_Classes_TeacherId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Classes");

            migrationBuilder.RenameColumn(
                name: "JoinedUtc",
                table: "ClassMembers",
                newName: "JoinedAtUtc");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "ClassMembers",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ClassMembers_StudentId",
                table: "ClassMembers",
                newName: "IX_ClassMembers_UserId");

            migrationBuilder.RenameColumn(
                name: "CreatedUtc",
                table: "Classes",
                newName: "CreatedAtUtc");

            migrationBuilder.AddColumn<short>(
                name: "RoleInClass",
                table: "ClassMembers",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "Status",
                table: "ClassMembers",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Classes",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Classes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Classes",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(64)",
                oldMaxLength: 64);

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Classes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Classes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Term",
                table: "Classes",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAtUtc",
                table: "Classes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassMembers_ClassId_RoleInClass",
                table: "ClassMembers",
                columns: new[] { "ClassId", "RoleInClass" });

            migrationBuilder.CreateIndex(
                name: "IX_Classes_OwnerId",
                table: "Classes",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_AspNetUsers_OwnerId",
                table: "Classes",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassMembers_AspNetUsers_UserId",
                table: "ClassMembers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
