namespace NexusERP.Domain.Entities.Purchases;

public class PurchaseOrderDetail
{
    public int Id { get; set; }
    public int PurchaseOrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public int ReceivedQty { get; set; }
    public DateTime CreatedAt { get; set; }

    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public Inventory.Product Product { get; set; } = null!;
}
