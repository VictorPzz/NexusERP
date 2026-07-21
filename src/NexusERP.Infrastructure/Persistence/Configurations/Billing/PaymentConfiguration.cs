using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusERP.Domain.Entities.Billing;

namespace NexusERP.Infrastructure.Persistence.Configurations.Billing;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(e => e.PaymentNumber).HasColumnName("payment_number").HasMaxLength(30).IsRequired();
        builder.Property(e => e.InvoiceId).HasColumnName("invoice_id").IsRequired();
        builder.Property(e => e.Amount).HasColumnName("amount").HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(e => e.PaymentMethod).HasColumnName("payment_method").HasMaxLength(20).IsRequired();
        builder.Property(e => e.PaymentDate).HasColumnName("payment_date").IsRequired();
        builder.Property(e => e.Reference).HasColumnName("reference").HasMaxLength(200);
        builder.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");

        builder.HasIndex(e => e.PaymentNumber).IsUnique().HasDatabaseName("ix_payments_number");
        builder.HasIndex(e => e.InvoiceId).HasDatabaseName("ix_payments_invoice_id");
        builder.HasIndex(e => e.PaymentDate).HasDatabaseName("ix_payments_date");

        builder.HasOne(e => e.Invoice).WithMany(e => e.Payments).HasForeignKey(e => e.InvoiceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.CreatedByUser).WithMany().HasForeignKey(e => e.CreatedBy).OnDelete(DeleteBehavior.Restrict);
    }
}
