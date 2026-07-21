using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusERP.Domain.Entities.Purchases;

namespace NexusERP.Infrastructure.Persistence.Configurations.Purchases;

public class PurchaseOrderDetailConfiguration : IEntityTypeConfiguration<PurchaseOrderDetail>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderDetail> builder)
    {
        builder.ToTable("purchase_order_details");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(e => e.PurchaseOrderId).HasColumnName("purchase_order_id").IsRequired();
        builder.Property(e => e.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(e => e.Quantity).HasColumnName("quantity").IsRequired();
        builder.Property(e => e.UnitPrice).HasColumnName("unit_price").HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(e => e.TaxRate).HasColumnName("tax_rate").HasColumnType("decimal(5,2)").HasDefaultValue(0);
        builder.Property(e => e.Subtotal).HasColumnName("subtotal").HasColumnType("decimal(12,2)").HasDefaultValue(0);
        builder.Property(e => e.TaxAmount).HasColumnName("tax_amount").HasColumnType("decimal(12,2)").HasDefaultValue(0);
        builder.Property(e => e.Total).HasColumnName("total").HasColumnType("decimal(12,2)").HasDefaultValue(0);
        builder.Property(e => e.ReceivedQty).HasColumnName("received_qty").HasDefaultValue(0);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()").ValueGeneratedOnAdd();

        builder.HasIndex(e => e.PurchaseOrderId).HasDatabaseName("ix_purchase_order_details_order_id");
        builder.HasIndex(e => e.ProductId).HasDatabaseName("ix_purchase_order_details_product_id");

        builder.HasOne(e => e.PurchaseOrder).WithMany(e => e.Details).HasForeignKey(e => e.PurchaseOrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Product).WithMany().HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Restrict);
    }
}
