using NexusERP.Domain.Common;
using NexusERP.Domain.Enums;

namespace NexusERP.Domain.Entities.Inventory;

public class Product : SoftDeleteEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.unit;
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal TaxRate { get; set; }
    public int MinStock { get; set; }
    public int? MaxStock { get; set; }
    public string? ImageUrl { get; set; }
    public string? Barcode { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsTaxable { get; set; } = true;

    public Category Category { get; set; } = null!;
    public ICollection<ProductSupplier> ProductSuppliers { get; set; } = new List<ProductSupplier>();
    public ICollection<Entities.Inventory.Inventory> Inventories { get; set; } = new List<Entities.Inventory.Inventory>();
    public ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
}
