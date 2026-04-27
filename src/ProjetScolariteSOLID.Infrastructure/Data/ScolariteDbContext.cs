using Microsoft.EntityFrameworkCore;
using ProjetScolariteSOLID.Infrastructure.Data.Configurations;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Infrastructure.Data;

/// <summary>
/// DbContext central de l'application — applique toutes les configurations via ApplyConfigurationsFromAssembly.
/// OCP : ajouter une entité = créer une IEntityTypeConfiguration sans toucher ce fichier.
/// </summary>
public sealed class ScolariteDbContext : DbContext
{
    public ScolariteDbContext(DbContextOptions<ScolariteDbContext> options) : base(options) { }

    public DbSet<Etudiant> Etudiants => Set<Etudiant>();
    public DbSet<Enseignant> Enseignants => Set<Enseignant>();
    public DbSet<Matiere> Matieres => Set<Matiere>();
    public DbSet<Classe> Classes => Set<Classe>();
    public DbSet<Inscription> Inscriptions => Set<Inscription>();
    public DbSet<Note> Notes => Set<Note>();

    // Référentiels
    public DbSet<Filiere> Filieres => Set<Filiere>();
    public DbSet<AnneeAcademique> AnneesAcademiques => Set<AnneeAcademique>();
    public DbSet<Niveau> Niveaux => Set<Niveau>();
    public DbSet<Specialite> Specialites => Set<Specialite>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<StatutInscriptionRef> StatutsInscription => Set<StatutInscriptionRef>();
    public DbSet<TypeEvaluationRef> TypesEvaluation => Set<TypeEvaluationRef>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // OCP — Découverte automatique de toutes les IEntityTypeConfiguration<T> de l'assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EtudiantConfiguration).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
