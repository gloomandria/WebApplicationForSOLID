using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Infrastructure.Data.Configurations;

public sealed class ClasseConfiguration : IEntityTypeConfiguration<Classe>
{
    public void Configure(EntityTypeBuilder<Classe> builder)
    {
        builder.ToTable("Classes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
               .UseIdentityColumn();

        builder.Property(c => c.Nom)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(c => c.CapaciteMax)
               .IsRequired()
               .HasDefaultValue(30);

        builder.HasOne(c => c.Niveau)
               .WithMany()
               .HasForeignKey(c => c.NiveauId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.AnneeAcademique)
               .WithMany()
               .HasForeignKey(c => c.AnneeAcademiqueId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Filiere)
               .WithMany()
               .HasForeignKey(c => c.FiliereId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.Nom, c.AnneeAcademiqueId })
               .IsUnique();
    }
}
