using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusERP.Domain.Entities.Inventory;

namespace NexusERP.Infrastructure.Persistence.Configurations.Inventory;

public class InventoryConfiguration : IEntityTypeConfiguration<Domain.Entities.Inventory.Inventory>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Inventory.Inventory> builder)
    {
        builder.ToTable("inventory");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(e => e.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(e => e.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(e => e.Quantity).HasColumnName("quantity").HasDefaultValue(0);
        builder.Property(e => e.ReservedQty).HasColumnName("reserved_qty").HasDefaultValue(0);
        builder.Property(e => e.LastCountDate).HasColumnName("last_count_date");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");

        builder.HasIndex(e => new { e.ProductId, e.WarehouseId }).IsUnique().HasDatabaseName("ix_inventory_product_warehouse");

        builder.HasOne(e => e.Product).WithMany(e => e.Inventories).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Warehouse).WithMany(e => e.Inventories).HasForeignKey(e => e.WarehouseId).OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(e => e.UpdatedAt);
    }
}
