using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusERP.Domain.Entities.People;

namespace NexusERP.Infrastructure.Persistence.Configurations.People;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("clients");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(e => e.PersonId).HasColumnName("person_id").IsRequired();
        builder.Property(e => e.ClientCode).HasColumnName("client_code").HasMaxLength(20).IsRequired();
        builder.Property(e => e.CreditLimit).HasColumnName("credit_limit").HasColumnType("decimal(12,2)").HasDefaultValue(0);
        builder.Property(e => e.CurrentBalance).HasColumnName("current_balance").HasColumnType("decimal(12,2)").HasDefaultValue(0);
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.CreatedBy).HasColumnName("created_by");
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasIndex(e => e.ClientCode).IsUnique().HasDatabaseName("ix_clients_code");
        builder.HasIndex(e => e.PersonId).IsUnique().HasDatabaseName("ix_clients_person_id");

        builder.HasOne(e => e.Person).WithOne(e => e.Client).HasForeignKey<Client>(e => e.PersonId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.Addresses).WithOne(e => e.Client).HasForeignKey(e => e.ClientId).OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(e => e.UpdatedAt);
    }
}
