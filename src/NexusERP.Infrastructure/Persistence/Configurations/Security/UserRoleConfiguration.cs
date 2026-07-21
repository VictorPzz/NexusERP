using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusERP.Domain.Entities.Security;

namespace NexusERP.Infrastructure.Persistence.Configurations.Security;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(e => e.RoleId).HasColumnName("role_id").IsRequired();
        builder.Property(e => e.AssignedAt).HasColumnName("assigned_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.AssignedBy).HasColumnName("assigned_by");

        builder.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique().HasDatabaseName("uq_user_roles");

        builder.HasOne(e => e.User).WithMany(e => e.UserRoles).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Role).WithMany(e => e.UserRoles).HasForeignKey(e => e.RoleId).OnDelete(DeleteBehavior.Cascade);
    }
}
