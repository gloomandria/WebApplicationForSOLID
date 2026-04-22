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

        builder.Property(e => e.Adresse)
               .HasMaxLength(500);

        builder.Property(e => e.DateNaissance)
               .IsRequired();

        builder.Property(e => e.DateInscription)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        // Lien obligatoire vers AspNetUsers
        builder.Property(e => e.UserId)
               .IsRequired()
               .HasMaxLength(450);

        builder.HasIndex(e => e.UserId)
               .IsUnique();

        builder.HasOne(e => e.User)
               .WithOne()
               .HasForeignKey<Etudiant>(e => e.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        // Propriétés calculées depuis User — non mappées en base
        builder.Ignore(e => e.Nom);
        builder.Ignore(e => e.Prenom);
        builder.Ignore(e => e.Email);
        builder.Ignore(e => e.Telephone);
        builder.Ignore(e => e.NomComplet);
    }
}
