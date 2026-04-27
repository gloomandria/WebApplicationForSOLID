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

        builder.Property(e => e.DateEmbauche)
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
               .HasForeignKey<Enseignant>(e => e.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

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

        // Propriétés calculées depuis User — non mappées en base
        builder.Ignore(e => e.Nom);
        builder.Ignore(e => e.Prenom);
        builder.Ignore(e => e.Email);
        builder.Ignore(e => e.Telephone);
        builder.Ignore(e => e.NomComplet);
    }
}
