namespace NexusERP.Domain.Entities.Inventory;

public class ProductSupplier
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int SupplierId { get; set; }
    public decimal SupplierPrice { get; set; }
    public int LeadTimeDays { get; set; }
    public int MinOrderQty { get; set; } = 1;
    public bool IsPreferred { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Product Product { get; set; } = null!;
    public People.Supplier Supplier { get; set; } = null!;
}
