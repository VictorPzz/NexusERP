using NexusERP.Domain.Common;
using NexusERP.Domain.Enums;

namespace NexusERP.Domain.Entities.Sales;

public class Order : SoftDeleteEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public OrderType OrderType { get; set; }
    public int? ClientId { get; set; }
    public int? SupplierId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.pending;
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public string? Notes { get; set; }

    public People.Client? Client { get; set; }
    public People.Supplier? Supplier { get; set; }
    public ICollection<OrderDetail> Details { get; set; } = new List<OrderDetail>();
}
