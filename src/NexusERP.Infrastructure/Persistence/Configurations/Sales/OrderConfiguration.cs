using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusERP.Domain.Entities.Sales;

namespace NexusERP.Infrastructure.Persistence.Configurations.Sales;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(e => e.OrderNumber).HasColumnName("order_number").HasMaxLength(30).IsRequired();
        builder.Property(e => e.OrderType).HasColumnName("order_type").HasMaxLength(20).IsRequired();
        builder.Property(e => e.ClientId).HasColumnName("client_id");
        builder.Property(e => e.SupplierId).HasColumnName("supplier_id");
        builder.Property(e => e.OrderDate).HasColumnName("order_date").IsRequired();
        builder.Property(e => e.ExpectedDate).HasColumnName("expected_date");
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
        builder.Property(e => e.Subtotal).HasColumnName("subtotal").HasColumnType("decimal(12,2)").HasDefaultValue(0);
        builder.Property(e => e.TaxAmount).HasColumnName("tax_amount").HasColumnType("decimal(12,2)").HasDefaultValue(0);
        builder.Property(e => e.Total).HasColumnName("total").HasColumnType("decimal(12,2)").HasDefaultValue(0);
        builder.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.CreatedBy).HasColumnName("created_by");
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasIndex(e => e.OrderNumber).IsUnique().HasDatabaseName("ix_orders_number");
        builder.HasIndex(e => e.ClientId).HasDatabaseName("ix_orders_client_id");
        builder.HasIndex(e => e.SupplierId).HasDatabaseName("ix_orders_supplier_id");
        builder.HasIndex(e => e.Status).HasDatabaseName("ix_orders_status");
        builder.HasIndex(e => e.OrderDate).HasDatabaseName("ix_orders_date");

        builder.HasOne(e => e.Client).WithMany().HasForeignKey(e => e.ClientId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Supplier).WithMany().HasForeignKey(e => e.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.Details).WithOne(e => e.Order).HasForeignKey(e => e.OrderId).OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(e => e.UpdatedAt);
    }
}
