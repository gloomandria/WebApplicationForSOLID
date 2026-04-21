using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Infrastructure.Data.Configurations;

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

        builder.Property(c => c.Niveau)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(30);

        builder.Property(c => c.AnneeAcademique)
               .IsRequired()
               .HasMaxLength(20);

        builder.Property(c => c.CapaciteMax)
               .IsRequired()
               .HasDefaultValue(30);

        builder.Property(c => c.Filiere)
               .IsRequired()
               .HasMaxLength(150);

        builder.HasIndex(c => new { c.Nom, c.AnneeAcademique })
               .IsUnique();
    }
}
