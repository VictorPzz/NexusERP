using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusERP.Domain.Entities.Security;

namespace NexusERP.Infrastructure.Persistence.Configurations.Security;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.Action).HasColumnName("action").HasMaxLength(50).IsRequired();
        builder.Property(e => e.EntityName).HasColumnName("entity_name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.EntityId).HasColumnName("entity_id");
        builder.Property(e => e.OldValues).HasColumnName("old_values").HasColumnType("jsonb");
        builder.Property(e => e.NewValues).HasColumnName("new_values").HasColumnType("jsonb");
        builder.Property(e => e.IpAddress).HasColumnName("ip_address").HasMaxLength(45);
        builder.Property(e => e.UserAgent).HasColumnName("user_agent").HasMaxLength(500);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");

        builder.HasIndex(e => e.UserId).HasDatabaseName("ix_audit_logs_user_id");
        builder.HasIndex(e => e.EntityName).HasDatabaseName("ix_audit_logs_entity_name");
        builder.HasIndex(e => e.EntityId).HasDatabaseName("ix_audit_logs_entity_id");
        builder.HasIndex(e => e.Action).HasDatabaseName("ix_audit_logs_action");
        builder.HasIndex(e => e.CreatedAt).HasDatabaseName("ix_audit_logs_created_at");

        builder.HasOne(e => e.User).WithMany(e => e.AuditLogs).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(e => e.UpdatedAt);
    }
}
