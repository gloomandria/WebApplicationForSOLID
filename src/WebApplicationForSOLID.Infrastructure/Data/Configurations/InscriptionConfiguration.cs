using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Infrastructure.Data.Configurations;

public sealed class InscriptionConfiguration : IEntityTypeConfiguration<Inscription>
{
    public void Configure(EntityTypeBuilder<Inscription> builder)
    {
        builder.ToTable("Inscriptions");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
               .UseIdentityColumn();

        builder.Property(i => i.Statut)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(30);

        builder.Property(i => i.DateInscription)
               .IsRequired()
               .HasDefaultValueSql("GETUTCDATE()");

        // Un étudiant ne peut être inscrit qu'une seule fois dans la même classe
        builder.HasIndex(i => new { i.EtudiantId, i.ClasseId })
               .IsUnique();

        builder.HasOne(i => i.Etudiant)
               .WithMany()
               .HasForeignKey(i => i.EtudiantId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Classe)
               .WithMany()
               .HasForeignKey(i => i.ClasseId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}
