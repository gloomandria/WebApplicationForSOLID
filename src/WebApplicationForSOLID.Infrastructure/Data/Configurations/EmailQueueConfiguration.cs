using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetScolariteSOLID.Domain.Models.Auth;

namespace ProjetScolariteSOLID.Infrastructure.Data.Configurations;

public sealed class EmailQueueConfiguration : IEntityTypeConfiguration<EmailQueue>
{
    public void Configure(EntityTypeBuilder<EmailQueue> builder)
    {
        builder.ToTable("EmailQueue");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Destinataire).HasMaxLength(256).IsRequired();
        builder.Property(e => e.Sujet).HasMaxLength(500).IsRequired();
        builder.Property(e => e.Corps).IsRequired();
        builder.Property(e => e.MessageErreur).HasMaxLength(2000);
        builder.Property(e => e.Statut).HasConversion<int>();
        builder.HasIndex(e => e.Statut);
        builder.HasIndex(e => e.DateCreation);
    }
}
