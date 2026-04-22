using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Infrastructure.Data.Configurations;

/// <summary>
/// SRP — Seule responsabilité : configurer le mapping EF Core de l'entité Etudiant.
/// Fluent API uniquement, aucune DataAnnotation de contrainte DB dans les entités.
/// </summary>
public sealed class EtudiantConfiguration : IEntityTypeConfiguration<Etudiant>
{
    public void Configure(EntityTypeBuilder<Etudiant> builder)
    {
        builder.ToTable("Etudiants");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .UseIdentityColumn();

        builder.Property(e => e.NumeroEtudiant)
               .IsRequired()
               .HasMaxLength(20);

        builder.HasIndex(e => e.NumeroEtudiant)
               .IsUnique();

        builder.Property(e => e.Nom)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(e => e.Prenom)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(e => e.Email)
               .IsRequired()
               .HasMaxLength(256);

        builder.HasIndex(e => e.Email)
               .IsUnique();

        builder.Property(e => e.Telephone)
               .HasMaxLength(20);

        builder.Property(e => e.Adresse)
               .HasMaxLength(500);

        builder.Property(e => e.DateNaissance)
               .IsRequired();

        builder.Property(e => e.DateInscription)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        // Propriété calculée — non mappée en base
        builder.Ignore(e => e.NomComplet);
    }
}
