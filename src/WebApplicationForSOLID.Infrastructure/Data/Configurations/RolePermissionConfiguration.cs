using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjetScolariteSOLID.Domain.Models.Auth;

namespace ProjetScolariteSOLID.Infrastructure.Data.Configurations;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("RolePermissions");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Ressource).HasMaxLength(100).IsRequired();
        builder.HasIndex(r => new { r.RoleId, r.Ressource }).IsUnique();

        builder.HasOne(r => r.Role)
               .WithMany()
               .HasForeignKey(r => r.RoleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
