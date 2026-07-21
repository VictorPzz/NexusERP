using NexusERP.Domain.Common;
using NexusERP.Domain.Enums;

namespace NexusERP.Domain.Entities.Inventory;

public class InventoryMovement : BaseEntity
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public MovementType MovementType { get; set; }
    public int Quantity { get; set; }
    public decimal? UnitCost { get; set; }
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }
    public string? Notes { get; set; }
    public int PerformedBy { get; set; }
    public DateTime MovementDate { get; set; }

    public Product Product { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
    public Security.User PerformedByUser { get; set; } = null!;
}
