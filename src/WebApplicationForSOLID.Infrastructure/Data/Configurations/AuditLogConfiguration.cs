using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Infrastructure.Data.Configurations;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.TableName)
               .IsRequired()
               .HasMaxLength(128);

        builder.Property(a => a.Action)
               .IsRequired()
               .HasMaxLength(16);

        builder.Property(a => a.KeyValues)
               .IsRequired()
               .HasMaxLength(256);

        builder.Property(a => a.OldValues)
               .HasColumnType("nvarchar(max)");

        builder.Property(a => a.NewValues)
               .HasColumnType("nvarchar(max)");

        builder.Property(a => a.UserId)
               .HasMaxLength(450);

        builder.Property(a => a.Timestamp)
               .IsRequired();

        // Index pour les requêtes fréquentes
        builder.HasIndex(a => a.TableName);
        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.Timestamp);
    }
}
