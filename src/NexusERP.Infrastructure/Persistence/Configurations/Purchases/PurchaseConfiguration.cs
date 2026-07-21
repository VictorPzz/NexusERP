using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusERP.Domain.Entities.Purchases;

namespace NexusERP.Infrastructure.Persistence.Configurations.Purchases;

public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.ToTable("purchases");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(e => e.PurchaseNumber).HasColumnName("purchase_number").HasMaxLength(30).IsRequired();
        builder.Property(e => e.SupplierId).HasColumnName("supplier_id").IsRequired();
        builder.Property(e => e.PurchaseOrderId).HasColumnName("purchase_order_id");
        builder.Property(e => e.PurchaseDate).HasColumnName("purchase_date").IsRequired();
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(e => e.Subtotal).HasColumnName("subtotal").HasColumnType("decimal(12,2)").HasDefaultValue(0);
        builder.Property(e => e.TaxAmount).HasColumnName("tax_amount").HasColumnType("decimal(12,2)").HasDefaultValue(0);
        builder.Property(e => e.Total).HasColumnName("total").HasColumnType("decimal(12,2)").HasDefaultValue(0);
        builder.Property(e => e.PaymentStatus).HasColumnName("payment_status").HasMaxLength(20).IsRequired();
        builder.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.CreatedBy).HasColumnName("created_by");
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasIndex(e => e.PurchaseNumber).IsUnique().HasDatabaseName("ix_purchases_number");
        builder.HasIndex(e => e.SupplierId).HasDatabaseName("ix_purchases_supplier_id");
        builder.HasIndex(e => e.PurchaseOrderId).HasDatabaseName("ix_purchases_order_id");
        builder.HasIndex(e => e.Status).HasDatabaseName("ix_purchases_status");
        builder.HasIndex(e => e.PurchaseDate).HasDatabaseName("ix_purchases_date");

        builder.HasOne(e => e.Supplier).WithMany().HasForeignKey(e => e.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.PurchaseOrder).WithMany().HasForeignKey(e => e.PurchaseOrderId).OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(e => e.Details).WithOne(e => e.Purchase).HasForeignKey(e => e.PurchaseId).OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(e => e.UpdatedAt);
    }
}
