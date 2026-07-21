namespace NexusERP.Domain.Entities.Sales;

public class SaleDetail
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal CostPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal DiscountRate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
    public decimal Profit { get; set; }
    public DateTime CreatedAt { get; set; }

    public Sale Sale { get; set; } = null!;
    public Inventory.Product Product { get; set; } = null!;
}
