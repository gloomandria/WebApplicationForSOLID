using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Infrastructure.Data.Configurations;

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

        builder.Property(e => e.Specialite)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(e => e.Grade)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(e => e.DateEmbauche)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        // Propriété calculée — non mappée en base
        builder.Ignore(e => e.NomComplet);
    }
}
