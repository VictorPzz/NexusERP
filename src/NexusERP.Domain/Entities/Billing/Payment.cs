using NexusERP.Domain.Common;
using NexusERP.Domain.Enums;

namespace NexusERP.Domain.Entities.Billing;

public class Payment : BaseEntity
{
    public string PaymentNumber { get; set; } = string.Empty;
    public int InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public DateTime PaymentDate { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public int CreatedBy { get; set; }

    public Invoice Invoice { get; set; } = null!;
    public Security.User CreatedByUser { get; set; } = null!;
}
