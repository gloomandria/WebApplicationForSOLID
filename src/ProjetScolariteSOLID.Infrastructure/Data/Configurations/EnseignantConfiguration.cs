using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Infrastructure.Data.Configurations;

public sealed class EnseignantConfiguration : IEntityTypeConfiguration<Enseignant>
{
    public void Configure(EntityTypeBuilder<Enseignant> builder)
    {
        builder.ToTable("Enseignants");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
               .UseIdentityColumn();

        builder.Property(e => e.Matricule)
               .IsRequired()
               .HasMaxLength(20);

        builder.HasIndex(e => e.Matricule)
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

        builder.Property(e => e.DateEmbauche)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(e => e.Specialite)
               .WithMany()
               .HasForeignKey(e => e.SpecialiteId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Grade)
               .WithMany()
               .HasForeignKey(e => e.GradeId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        // Propriété calculée — non mappée en base
        builder.Ignore(e => e.NomComplet);
    }
}
