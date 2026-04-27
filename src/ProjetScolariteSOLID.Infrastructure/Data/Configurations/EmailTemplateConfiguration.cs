using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetScolariteSOLID.Domain.Models;

namespace ProjetScolariteSOLID.Infrastructure.Data.Configurations;

public sealed class EmailTemplateConfiguration : IEntityTypeConfiguration<EmailTemplate>
{
    public void Configure(EntityTypeBuilder<EmailTemplate> builder)
    {
        builder.ToTable("EmailTemplates");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Code).HasMaxLength(100).IsRequired();
        builder.HasIndex(e => e.Code).IsUnique();
        builder.Property(e => e.Nom).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Sujet).HasMaxLength(500).IsRequired();
        builder.Property(e => e.Corps).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(500);
    }
}
