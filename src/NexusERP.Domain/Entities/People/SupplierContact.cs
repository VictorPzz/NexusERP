namespace NexusERP.Domain.Entities.People;

public class SupplierContact
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public string ContactType { get; set; } = string.Empty;
    public string ContactValue { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime CreatedAt { get; set; }

    public Supplier Supplier { get; set; } = null!;
}
