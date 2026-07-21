using NexusERP.Domain.Common;

namespace NexusERP.Domain.Entities.Inventory;

public class Inventory : BaseEntity
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public int Quantity { get; set; }
    public int ReservedQty { get; set; }
    public DateTime? LastCountDate { get; set; }

    public Product Product { get; set; } = null!;
    public Warehouse Warehouse { get; set; } = null!;
}
