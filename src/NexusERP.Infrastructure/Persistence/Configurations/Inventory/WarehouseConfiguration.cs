using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusERP.Domain.Entities.Inventory;

namespace NexusERP.Infrastructure.Persistence.Configurations.Inventory;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("warehouses");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        builder.Property(e => e.Code).HasColumnName("code").HasMaxLength(20).IsRequired();
        builder.Property(e => e.Address).HasColumnName("address").HasMaxLength(500);
        builder.Property(e => e.ManagerId).HasColumnName("manager_id");
        builder.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(e => e.IsDefault).HasColumnName("is_default").HasDefaultValue(false);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");

        builder.HasIndex(e => e.Code).IsUnique().HasDatabaseName("ix_warehouses_code");

        builder.HasMany(e => e.Inventories).WithOne(e => e.Warehouse).HasForeignKey(e => e.WarehouseId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.InventoryMovements).WithOne(e => e.Warehouse).HasForeignKey(e => e.WarehouseId).OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(e => e.UpdatedAt);
    }
}
