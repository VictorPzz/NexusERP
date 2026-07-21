using NexusERP.Domain.Common;
using NexusERP.Domain.Enums;

namespace NexusERP.Domain.Entities.Billing;

public class Invoice : SoftDeleteEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public InvoiceType InvoiceType { get; set; } = InvoiceType.sale;
    public int? SaleId { get; set; }
    public int? PurchaseId { get; set; }
    public int? ClientId { get; set; }
    public int? SupplierId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime? DueDate { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.issued;
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public string? Notes { get; set; }

    public Sales.Sale? Sale { get; set; }
    public Purchases.Purchase? Purchase { get; set; }
    public People.Client? Client { get; set; }
    public People.Supplier? Supplier { get; set; }
    public ICollection<InvoiceDetail> Details { get; set; } = new List<InvoiceDetail>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
