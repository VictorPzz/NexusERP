using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusERP.Domain.Entities.Inventory;

namespace NexusERP.Infrastructure.Persistence.Configurations.Inventory;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(e => e.Code).HasColumnName("code").HasMaxLength(50).IsRequired();
        builder.Property(e => e.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.Description).HasColumnName("description").HasMaxLength(1000);
        builder.Property(e => e.CategoryId).HasColumnName("category_id").IsRequired();
        builder.Property(e => e.UnitOfMeasure).HasColumnName("unit_of_measure").HasMaxLength(20).IsRequired();
        builder.Property(e => e.CostPrice).HasColumnName("cost_price").HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(e => e.SellingPrice).HasColumnName("selling_price").HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(e => e.TaxRate).HasColumnName("tax_rate").HasColumnType("decimal(5,2)").HasDefaultValue(0);
        builder.Property(e => e.MinStock).HasColumnName("min_stock").HasDefaultValue(0);
        builder.Property(e => e.MaxStock).HasColumnName("max_stock");
        builder.Property(e => e.ImageUrl).HasColumnName("image_url").HasMaxLength(500);
        builder.Property(e => e.Barcode).HasColumnName("barcode").HasMaxLength(100);
        builder.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(e => e.IsTaxable).HasColumnName("is_taxable").HasDefaultValue(true);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
        builder.Property(e => e.CreatedBy).HasColumnName("created_by");
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasIndex(e => e.Code).IsUnique().HasDatabaseName("ix_products_code");
        builder.HasIndex(e => e.Barcode).IsUnique().HasDatabaseName("ix_products_barcode").HasFilter("barcode IS NOT NULL");
        builder.HasIndex(e => e.CategoryId).HasDatabaseName("ix_products_category_id");

        builder.HasOne(e => e.Category).WithMany(e => e.Products).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(e => e.ProductSuppliers).WithOne(e => e.Product).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.Inventories).WithOne(e => e.Product).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(e => e.InventoryMovements).WithOne(e => e.Product).HasForeignKey(e => e.ProductId).OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(e => e.UpdatedAt);
    }
}
