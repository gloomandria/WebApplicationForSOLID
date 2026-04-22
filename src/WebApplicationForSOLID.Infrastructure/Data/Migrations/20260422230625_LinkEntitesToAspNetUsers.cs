using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetScolariteSOLID.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class LinkEntitesToAspNetUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Etudiants_Email",
                table: "Etudiants");

            migrationBuilder.DropIndex(
                name: "IX_Enseignants_Email",
                table: "Enseignants");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Etudiants");

            migrationBuilder.DropColumn(
                name: "Nom",
                table: "Etudiants");

            migrationBuilder.DropColumn(
                name: "Prenom",
                table: "Etudiants");

            migrationBuilder.DropColumn(
                name: "Telephone",
                table: "Etudiants");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Enseignants");

            migrationBuilder.DropColumn(
                name: "Nom",
                table: "Enseignants");

            migrationBuilder.DropColumn(
                name: "Prenom",
                table: "Enseignants");

            migrationBuilder.DropColumn(
                name: "Telephone",
                table: "Enseignants");

            // Remove seeded rows (they will be re-created by the DataSeeder with proper UserId links).
            // Also remove associated Identity accounts so the seeder can recreate them cleanly.
            // Delete in dependency order to satisfy FK constraints.
            migrationBuilder.Sql("DELETE FROM Notes");
            migrationBuilder.Sql("DELETE FROM Inscriptions");
            migrationBuilder.Sql("DELETE FROM Etudiants");
            migrationBuilder.Sql("DELETE FROM Enseignants");
            migrationBuilder.Sql("DELETE FROM AspNetUserRoles WHERE UserId IN (SELECT Id FROM AspNetUsers WHERE Email LIKE '%@ecole.fr')");
            migrationBuilder.Sql("DELETE FROM AspNetUsers WHERE Email LIKE '%@ecole.fr'");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Etudiants",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Enseignants",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Etudiants_UserId",
                table: "Etudiants",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enseignants_UserId",
                table: "Enseignants",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Enseignants_AspNetUsers_UserId",
                table: "Enseignants",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Etudiants_AspNetUsers_UserId",
                table: "Etudiants",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enseignants_AspNetUsers_UserId",
                table: "Enseignants");

            migrationBuilder.DropForeignKey(
                name: "FK_Etudiants_AspNetUsers_UserId",
                table: "Etudiants");

            migrationBuilder.DropIndex(
                name: "IX_Etudiants_UserId",
                table: "Etudiants");

            migrationBuilder.DropIndex(
                name: "IX_Enseignants_UserId",
                table: "Enseignants");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Etudiants");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Enseignants");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Etudiants",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nom",
                table: "Etudiants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Prenom",
                table: "Etudiants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telephone",
                table: "Etudiants",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Enseignants",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nom",
                table: "Enseignants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Prenom",
                table: "Enseignants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Telephone",
                table: "Enseignants",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Etudiants_Email",
                table: "Etudiants",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enseignants_Email",
                table: "Enseignants",
                column: "Email",
                unique: true);
        }
    }
}
