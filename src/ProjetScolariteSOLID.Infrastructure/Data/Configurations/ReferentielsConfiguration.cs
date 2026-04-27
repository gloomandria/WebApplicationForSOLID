using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Infrastructure.Data.Configurations;

public sealed class FiliereConfiguration : IEntityTypeConfiguration<Filiere>
{
    public void Configure(EntityTypeBuilder<Filiere> builder)
    {
        builder.ToTable("Filieres");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).UseIdentityColumn();
        builder.Property(f => f.Libelle).IsRequired().HasMaxLength(150);
        builder.HasIndex(f => f.Libelle).IsUnique();
    }
}

public sealed class AnneeAcademiqueConfiguration : IEntityTypeConfiguration<AnneeAcademique>
{
    public void Configure(EntityTypeBuilder<AnneeAcademique> builder)
    {
        builder.ToTable("AnneesAcademiques");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).UseIdentityColumn();
        builder.Property(a => a.Libelle).IsRequired().HasMaxLength(20);
        builder.HasIndex(a => a.Libelle).IsUnique();
    }
}

public sealed class NiveauConfiguration : IEntityTypeConfiguration<Niveau>
{
    public void Configure(EntityTypeBuilder<Niveau> builder)
    {
        builder.ToTable("Niveaux");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).UseIdentityColumn();
        builder.Property(n => n.Libelle).IsRequired().HasMaxLength(50);
        builder.HasIndex(n => n.Libelle).IsUnique();
    }
}

public sealed class SpecialiteConfiguration : IEntityTypeConfiguration<Specialite>
{
    public void Configure(EntityTypeBuilder<Specialite> builder)
    {
        builder.ToTable("Specialites");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).UseIdentityColumn();
        builder.Property(s => s.Libelle).IsRequired().HasMaxLength(200);
        builder.HasIndex(s => s.Libelle).IsUnique();
    }
}

public sealed class GradeConfiguration : IEntityTypeConfiguration<Grade>
{
    public void Configure(EntityTypeBuilder<Grade> builder)
    {
        builder.ToTable("Grades");
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id).UseIdentityColumn();
        builder.Property(g => g.Libelle).IsRequired().HasMaxLength(100);
        builder.HasIndex(g => g.Libelle).IsUnique();
    }
}

public sealed class StatutInscriptionRefConfiguration : IEntityTypeConfiguration<StatutInscriptionRef>
{
    public void Configure(EntityTypeBuilder<StatutInscriptionRef> builder)
    {
        builder.ToTable("StatutsInscription");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).UseIdentityColumn();
        builder.Property(s => s.Libelle).IsRequired().HasMaxLength(50);
        builder.HasIndex(s => s.Libelle).IsUnique();
    }
}

public sealed class TypeEvaluationRefConfiguration : IEntityTypeConfiguration<TypeEvaluationRef>
{
    public void Configure(EntityTypeBuilder<TypeEvaluationRef> builder)
    {
        builder.ToTable("TypesEvaluation");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).UseIdentityColumn();
        builder.Property(t => t.Libelle).IsRequired().HasMaxLength(100);
        builder.HasIndex(t => t.Libelle).IsUnique();
    }
}
