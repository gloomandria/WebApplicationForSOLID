using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplicationForSOLID.Domain.Models;

namespace WebApplicationForSOLID.Infrastructure.Data.Configurations;

public sealed class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.ToTable("Notes");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
               .UseIdentityColumn();

        builder.Property(n => n.Valeur)
               .IsRequired()
               .HasPrecision(4, 2); // ex: 18.50

        builder.Property(n => n.TypeEvaluation)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(n => n.Date)
               .IsRequired();

        builder.Property(n => n.Commentaire)
               .HasMaxLength(500);

        builder.HasOne(n => n.Etudiant)
               .WithMany()
               .HasForeignKey(n => n.EtudiantId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.Matiere)
               .WithMany()
               .HasForeignKey(n => n.MatiereId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);
    }
}
