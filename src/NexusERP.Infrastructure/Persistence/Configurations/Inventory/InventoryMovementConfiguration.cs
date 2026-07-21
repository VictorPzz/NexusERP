using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusERP.Domain.Entities.Inventory;

namespace NexusERP.Infrastructure.Persistence.Configurations.Inventory;

public class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> builder)
    {
        builder.ToTable("inventory_movements");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(e => e.ProductId).HasColumnName("product_id").IsRequired();
        builder.Property(e => e.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(e => e.MovementType).HasColumnName("movement_type").HasMaxLength(20).IsRequired();
        builder.Property(e => e.Quantity).HasColumnName("quantity").IsRequired();
        builder.Property(e => e.UnitCost).HasColumnName("unit_cost").HasColumnType("decimal(12,2)");
        builder.Property(e => e.ReferenceType).HasColumnName("reference_type").HasMaxLength(50);
        builder.Property(e => e.ReferenceId).HasColumnName("reference_id");
        builder.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(e => e.PerformedBy).HasColumnName("performed_by").IsRequired();
        builder.Property(e => e.MovementDate).HasColumnName("movement_date").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");

        builder.HasIndex(e => e.ProductId).HasDatabaseName("ix_inventory_movements_product_id");
        builder.HasIndex(e => e.WarehouseId).HasDatabaseName("ix_inventory_movements_warehouse_id");
        builder.HasIndex(e => e.MovementDate).HasDatabaseName("ix_inventory_movements_date");

        builder.HasOne(e => e.Product).WithMany(e => e.InventoryMovements).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Warehouse).WithMany(e => e.InventoryMovements).HasForeignKey(e => e.WarehouseId).OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(e => e.UpdatedAt);
    }
}
