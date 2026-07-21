using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusERP.Domain.Entities.Security;

namespace NexusERP.Infrastructure.Persistence.Configurations.Security;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(e => e.Token).HasColumnName("token").HasMaxLength(500).IsRequired();
        builder.Property(e => e.ExpiresAt).HasColumnName("expires_at").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.RevokedAt).HasColumnName("revoked_at");
        builder.Property(e => e.ReplacedBy).HasColumnName("replaced_by");
        builder.Property(e => e.IsRevoked).HasColumnName("is_revoked").HasDefaultValue(false);

        builder.HasIndex(e => e.Token).IsUnique().HasDatabaseName("uq_refresh_tokens_token");
        builder.HasIndex(e => e.UserId).HasDatabaseName("ix_refresh_tokens_user_id");
        builder.HasIndex(e => e.ExpiresAt).HasDatabaseName("ix_refresh_tokens_expires_at");
        builder.HasIndex(e => e.IsRevoked).HasDatabaseName("ix_refresh_tokens_is_revoked");

        builder.HasOne(e => e.User).WithMany(e => e.RefreshTokens).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(e => e.UpdatedAt);
    }
}
