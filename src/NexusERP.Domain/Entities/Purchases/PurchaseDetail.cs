namespace NexusERP.Domain.Entities.Purchases;

public class PurchaseDetail
{
    public int Id { get; set; }
    public int PurchaseId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }

    public Purchase Purchase { get; set; } = null!;
    public Inventory.Product Product { get; set; } = null!;
}
