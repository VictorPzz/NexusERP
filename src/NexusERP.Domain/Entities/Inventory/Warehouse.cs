using NexusERP.Domain.Common;

namespace NexusERP.Domain.Entities.Inventory;

public class Warehouse : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int? ManagerId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; }

    public ICollection<Entities.Inventory.Inventory> Inventories { get; set; } = new List<Entities.Inventory.Inventory>();
    public ICollection<InventoryMovement> InventoryMovements { get; set; } = new List<InventoryMovement>();
}
