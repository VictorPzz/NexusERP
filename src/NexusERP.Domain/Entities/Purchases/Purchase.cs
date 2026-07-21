using NexusERP.Domain.Common;
using NexusERP.Domain.Enums;

namespace NexusERP.Domain.Entities.Purchases;

public class Purchase : SoftDeleteEntity
{
    public string PurchaseNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public int? PurchaseOrderId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public PurchaseStatus Status { get; set; } = PurchaseStatus.pending;
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.pending;
    public string? Notes { get; set; }

    public People.Supplier Supplier { get; set; } = null!;
    public PurchaseOrder? PurchaseOrder { get; set; }
    public ICollection<PurchaseDetail> Details { get; set; } = new List<PurchaseDetail>();
}
