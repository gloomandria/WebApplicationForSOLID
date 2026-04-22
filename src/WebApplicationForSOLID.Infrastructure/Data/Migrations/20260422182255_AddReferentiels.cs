using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetScolariteSOLID.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReferentiels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Classes_Nom_AnneeAcademique",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "TypeEvaluation",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "Inscriptions");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "Enseignants");

            migrationBuilder.DropColumn(
                name: "Specialite",
                table: "Enseignants");

            migrationBuilder.DropColumn(
                name: "AnneeAcademique",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Filiere",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Niveau",
                table: "Classes");

            migrationBuilder.AddColumn<int>(
                name: "TypeEvaluationId",
                table: "Notes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatutId",
                table: "Inscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GradeId",
                table: "Enseignants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpecialiteId",
                table: "Enseignants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AnneeAcademiqueId",
                table: "Classes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FiliereId",
                table: "Classes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NiveauId",
                table: "Classes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AnneesAcademiques",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Libelle = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnneesAcademiques", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Filieres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Libelle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filieres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Libelle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Niveaux",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Libelle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Niveaux", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Specialites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Libelle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StatutsInscription",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Libelle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatutsInscription", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypesEvaluation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Libelle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypesEvaluation", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notes_TypeEvaluationId",
                table: "Notes",
                column: "TypeEvaluationId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscriptions_StatutId",
                table: "Inscriptions",
                column: "StatutId");

            migrationBuilder.CreateIndex(
                name: "IX_Enseignants_GradeId",
                table: "Enseignants",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Enseignants_SpecialiteId",
                table: "Enseignants",
                column: "SpecialiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_AnneeAcademiqueId",
                table: "Classes",
                column: "AnneeAcademiqueId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_FiliereId",
                table: "Classes",
                column: "FiliereId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_NiveauId",
                table: "Classes",
                column: "NiveauId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_Nom_AnneeAcademiqueId",
                table: "Classes",
                columns: new[] { "Nom", "AnneeAcademiqueId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnneesAcademiques_Libelle",
                table: "AnneesAcademiques",
                column: "Libelle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Filieres_Libelle",
                table: "Filieres",
                column: "Libelle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Grades_Libelle",
                table: "Grades",
                column: "Libelle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Niveaux_Libelle",
                table: "Niveaux",
                column: "Libelle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specialites_Libelle",
                table: "Specialites",
                column: "Libelle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatutsInscription_Libelle",
                table: "StatutsInscription",
                column: "Libelle",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TypesEvaluation_Libelle",
                table: "TypesEvaluation",
                column: "Libelle",
                unique: true);

            // Seed reference data
            migrationBuilder.InsertData("AnneesAcademiques", new[] { "Libelle" }, new object[] { "2023-2024" });
            migrationBuilder.InsertData("AnneesAcademiques", new[] { "Libelle" }, new object[] { "2024-2025" });
            migrationBuilder.InsertData("AnneesAcademiques", new[] { "Libelle" }, new object[] { "2025-2026" });

            migrationBuilder.InsertData("Filieres", new[] { "Libelle" }, new object[] { "Informatique" });
            migrationBuilder.InsertData("Filieres", new[] { "Libelle" }, new object[] { "Mathématiques" });
            migrationBuilder.InsertData("Filieres", new[] { "Libelle" }, new object[] { "Physique" });

            migrationBuilder.InsertData("Niveaux", new[] { "Libelle" }, new object[] { "Licence 1" });
            migrationBuilder.InsertData("Niveaux", new[] { "Libelle" }, new object[] { "Licence 2" });
            migrationBuilder.InsertData("Niveaux", new[] { "Libelle" }, new object[] { "Licence 3" });
            migrationBuilder.InsertData("Niveaux", new[] { "Libelle" }, new object[] { "Master 1" });
            migrationBuilder.InsertData("Niveaux", new[] { "Libelle" }, new object[] { "Master 2" });

            migrationBuilder.InsertData("Specialites", new[] { "Libelle" }, new object[] { "Génie Logiciel" });
            migrationBuilder.InsertData("Specialites", new[] { "Libelle" }, new object[] { "Réseaux et Télécommunications" });
            migrationBuilder.InsertData("Specialites", new[] { "Libelle" }, new object[] { "Intelligence Artificielle" });
            migrationBuilder.InsertData("Specialites", new[] { "Libelle" }, new object[] { "Base de Données" });

            migrationBuilder.InsertData("Grades", new[] { "Libelle" }, new object[] { "Assistant" });
            migrationBuilder.InsertData("Grades", new[] { "Libelle" }, new object[] { "Maître Assistant" });
            migrationBuilder.InsertData("Grades", new[] { "Libelle" }, new object[] { "Maître de Conférences" });
            migrationBuilder.InsertData("Grades", new[] { "Libelle" }, new object[] { "Professeur" });

            migrationBuilder.InsertData("StatutsInscription", new[] { "Libelle" }, new object[] { "Active" });
            migrationBuilder.InsertData("StatutsInscription", new[] { "Libelle" }, new object[] { "Annulée" });
            migrationBuilder.InsertData("StatutsInscription", new[] { "Libelle" }, new object[] { "En attente" });

            migrationBuilder.InsertData("TypesEvaluation", new[] { "Libelle" }, new object[] { "Examen" });
            migrationBuilder.InsertData("TypesEvaluation", new[] { "Libelle" }, new object[] { "Contrôle Continu" });
            migrationBuilder.InsertData("TypesEvaluation", new[] { "Libelle" }, new object[] { "TP" });
            migrationBuilder.InsertData("TypesEvaluation", new[] { "Libelle" }, new object[] { "Projet" });

            // Update existing rows to point to the first valid FK value (Id=1)
            migrationBuilder.Sql("UPDATE Classes SET AnneeAcademiqueId = 1, FiliereId = 1, NiveauId = 1 WHERE AnneeAcademiqueId = 0 OR FiliereId = 0 OR NiveauId = 0");
            migrationBuilder.Sql("UPDATE Enseignants SET SpecialiteId = 1, GradeId = 1 WHERE SpecialiteId = 0 OR GradeId = 0");
            migrationBuilder.Sql("UPDATE Inscriptions SET StatutId = 1 WHERE StatutId = 0");
            migrationBuilder.Sql("UPDATE Notes SET TypeEvaluationId = 1 WHERE TypeEvaluationId = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_AnneesAcademiques_AnneeAcademiqueId",
                table: "Classes",
                column: "AnneeAcademiqueId",
                principalTable: "AnneesAcademiques",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Filieres_FiliereId",
                table: "Classes",
                column: "FiliereId",
                principalTable: "Filieres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Niveaux_NiveauId",
                table: "Classes",
                column: "NiveauId",
                principalTable: "Niveaux",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enseignants_Grades_GradeId",
                table: "Enseignants",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enseignants_Specialites_SpecialiteId",
                table: "Enseignants",
                column: "SpecialiteId",
                principalTable: "Specialites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Inscriptions_StatutsInscription_StatutId",
                table: "Inscriptions",
                column: "StatutId",
                principalTable: "StatutsInscription",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_TypesEvaluation_TypeEvaluationId",
                table: "Notes",
                column: "TypeEvaluationId",
                principalTable: "TypesEvaluation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_AnneesAcademiques_AnneeAcademiqueId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Filieres_FiliereId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Niveaux_NiveauId",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_Enseignants_Grades_GradeId",
                table: "Enseignants");

            migrationBuilder.DropForeignKey(
                name: "FK_Enseignants_Specialites_SpecialiteId",
                table: "Enseignants");

            migrationBuilder.DropForeignKey(
                name: "FK_Inscriptions_StatutsInscription_StatutId",
                table: "Inscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Notes_TypesEvaluation_TypeEvaluationId",
                table: "Notes");

            migrationBuilder.DropTable(
                name: "AnneesAcademiques");

            migrationBuilder.DropTable(
                name: "Filieres");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "Niveaux");

            migrationBuilder.DropTable(
                name: "Specialites");

            migrationBuilder.DropTable(
                name: "StatutsInscription");

            migrationBuilder.DropTable(
                name: "TypesEvaluation");

            migrationBuilder.DropIndex(
                name: "IX_Notes_TypeEvaluationId",
                table: "Notes");

            migrationBuilder.DropIndex(
                name: "IX_Inscriptions_StatutId",
                table: "Inscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Enseignants_GradeId",
                table: "Enseignants");

            migrationBuilder.DropIndex(
                name: "IX_Enseignants_SpecialiteId",
                table: "Enseignants");

            migrationBuilder.DropIndex(
                name: "IX_Classes_AnneeAcademiqueId",
                table: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Classes_FiliereId",
                table: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Classes_NiveauId",
                table: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Classes_Nom_AnneeAcademiqueId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "TypeEvaluationId",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "StatutId",
                table: "Inscriptions");

            migrationBuilder.DropColumn(
                name: "GradeId",
                table: "Enseignants");

            migrationBuilder.DropColumn(
                name: "SpecialiteId",
                table: "Enseignants");

            migrationBuilder.DropColumn(
                name: "AnneeAcademiqueId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "FiliereId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "NiveauId",
                table: "Classes");

            migrationBuilder.AddColumn<string>(
                name: "TypeEvaluation",
                table: "Notes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Statut",
                table: "Inscriptions",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Grade",
                table: "Enseignants",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Specialite",
                table: "Enseignants",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AnneeAcademique",
                table: "Classes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Filiere",
                table: "Classes",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Niveau",
                table: "Classes",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_Nom_AnneeAcademique",
                table: "Classes",
                columns: new[] { "Nom", "AnneeAcademique" },
                unique: true);
        }
    }
}
