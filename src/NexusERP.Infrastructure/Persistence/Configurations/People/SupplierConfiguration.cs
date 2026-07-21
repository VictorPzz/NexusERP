using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusERP.Domain.Entities.People;

namespace NexusERP.Infrastructure.Persistence.Configurations.People;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("suppliers");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(e => e.PersonId).HasColumnName("person_id").IsRequired();
        builder.Property(e => e.SupplierCode).HasColumnName("supplier_code").HasMaxLength(20).IsRequired();
        builder.Property(e => e.CompanyName).HasColumnName("company_name").HasMaxLength(200);
        builder.Property(e => e.TaxId).HasColumnName("tax_id").HasMaxLength(20);
        builder.Property(e => e.Website).HasColumnName("website").HasMaxLength(255);
        builder.Property(e => e.PaymentTerms).HasColumnName("payment_terms").HasMaxLength(255);
        builder.Property(e => e.Rating).HasColumnName("rating").HasColumnType("decimal(3,2)");
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.CreatedBy).HasColumnName("created_by");
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasIndex(e => e.SupplierCode).IsUnique().HasDatabaseName("ix_suppliers_code");
        builder.HasIndex(e => e.PersonId).IsUnique().HasDatabaseName("ix_suppliers_person_id");

        builder.HasOne(e => e.Person).WithOne(e => e.Supplier).HasForeignKey<Supplier>(e => e.PersonId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.Contacts).WithOne(e => e.Supplier).HasForeignKey(e => e.SupplierId).OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(e => e.UpdatedAt);
    }
}
