using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Infrastructure.Data.Configurations;

public sealed class MatiereConfiguration : IEntityTypeConfiguration<Matiere>
{
    public void Configure(EntityTypeBuilder<Matiere> builder)
    {
        builder.ToTable("Matieres");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
               .UseIdentityColumn();

        builder.Property(m => m.Code)
               .IsRequired()
               .HasMaxLength(20);

        builder.HasIndex(m => m.Code)
               .IsUnique();

        builder.Property(m => m.Intitule)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(m => m.Description)
               .HasMaxLength(1000);

        builder.Property(m => m.Coefficient)
               .IsRequired()
               .HasDefaultValue(1);

        builder.Property(m => m.VolumeHoraire)
               .IsRequired()
               .HasDefaultValue(0);

        // Clé étrangère vers Enseignant — nullable (matière sans enseignant autorisée)
        builder.HasOne(m => m.Enseignant)
               .WithMany()
               .HasForeignKey(m => m.EnseignantId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
