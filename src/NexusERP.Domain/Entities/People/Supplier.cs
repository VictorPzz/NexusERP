using NexusERP.Domain.Common;
using NexusERP.Domain.Enums;

namespace NexusERP.Domain.Entities.People;

public class Supplier : SoftDeleteEntity
{
    public int PersonId { get; set; }
    public string SupplierCode { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? TaxId { get; set; }
    public string? Website { get; set; }
    public string? PaymentTerms { get; set; }
    public decimal? Rating { get; set; }
    public SupplierStatus Status { get; set; } = SupplierStatus.active;
    public string? Notes { get; set; }

    public Person Person { get; set; } = null!;
    public ICollection<SupplierContact> Contacts { get; set; } = new List<SupplierContact>();
}
