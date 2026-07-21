using NexusERP.Domain.Common;
using NexusERP.Domain.Enums;

namespace NexusERP.Domain.Entities.Sales;

public class Sale : SoftDeleteEntity
{
    public string SaleNumber { get; set; } = string.Empty;
    public int ClientId { get; set; }
    public DateTime SaleDate { get; set; }
    public SaleStatus Status { get; set; } = SaleStatus.pending;
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.pending;
    public string? Notes { get; set; }

    public People.Client Client { get; set; } = null!;
    public ICollection<SaleDetail> Details { get; set; } = new List<SaleDetail>();
}
