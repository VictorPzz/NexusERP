using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusERP.Domain.Entities.Billing;

namespace NexusERP.Infrastructure.Persistence.Configurations.Billing;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("invoices");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(e => e.InvoiceNumber).HasColumnName("invoice_number").HasMaxLength(30).IsRequired();
        builder.Property(e => e.InvoiceType).HasColumnName("invoice_type").HasMaxLength(20).IsRequired();
        builder.Property(e => e.SaleId).HasColumnName("sale_id");
        builder.Property(e => e.PurchaseId).HasColumnName("purchase_id");
        builder.Property(e => e.ClientId).HasColumnName("client_id");
        builder.Property(e => e.SupplierId).HasColumnName("supplier_id");
        builder.Property(e => e.InvoiceDate).HasColumnName("invoice_date").IsRequired();
        builder.Property(e => e.DueDate).HasColumnName("due_date");
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(e => e.Subtotal).HasColumnName("subtotal").HasColumnType("decimal(12,2)").HasDefaultValue(0);
        builder.Property(e => e.TaxAmount).HasColumnName("tax_amount").HasColumnType("decimal(12,2)").HasDefaultValue(0);
        builder.Property(e => e.Total).HasColumnName("total").HasColumnType("decimal(12,2)").HasDefaultValue(0);
        builder.Property(e => e.AmountPaid).HasColumnName("amount_paid").HasColumnType("decimal(12,2)").HasDefaultValue(0);
        builder.Property(e => e.BalanceDue).HasColumnName("balance_due").HasColumnType("decimal(12,2)").HasDefaultValue(0);
        builder.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.CreatedBy).HasColumnName("created_by");
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasIndex(e => e.InvoiceNumber).IsUnique().HasDatabaseName("ix_invoices_number");
        builder.HasIndex(e => e.InvoiceType).HasDatabaseName("ix_invoices_type");
        builder.HasIndex(e => e.Status).HasDatabaseName("ix_invoices_status");
        builder.HasIndex(e => e.InvoiceDate).HasDatabaseName("ix_invoices_date");
        builder.HasIndex(e => e.ClientId).HasDatabaseName("ix_invoices_client_id");
        builder.HasIndex(e => e.SupplierId).HasDatabaseName("ix_invoices_supplier_id");

        builder.HasOne(e => e.Sale).WithMany().HasForeignKey(e => e.SaleId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Purchase).WithMany().HasForeignKey(e => e.PurchaseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Client).WithMany().HasForeignKey(e => e.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Supplier).WithMany().HasForeignKey(e => e.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.Details).WithOne(e => e.Invoice).HasForeignKey(e => e.InvoiceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.Payments).WithOne(e => e.Invoice).HasForeignKey(e => e.InvoiceId).OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(e => e.UpdatedAt);
    }
}
