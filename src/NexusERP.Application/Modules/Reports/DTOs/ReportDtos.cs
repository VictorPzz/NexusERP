namespace NexusERP.Application.Modules.Reports.DTOs;

public class ReportFilters
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? GroupBy { get; set; }
}

public class SalesReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalSales { get; set; }
    public int TotalTransactions { get; set; }
    public decimal AverageSale { get; set; }
    public decimal TotalTax { get; set; }
    public List<SalesReportGroupDto> Groups { get; set; } = new();
    public List<TopClientDto> TopClients { get; set; } = new();
}

public class SalesReportGroupDto
{
    public string Period { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public decimal Total { get; set; }
    public decimal Tax { get; set; }
}

public class TopClientDto
{
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public int PurchaseCount { get; set; }
    public decimal TotalSpent { get; set; }
}

public class PurchasesReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalPurchases { get; set; }
    public int TotalTransactions { get; set; }
    public decimal AveragePurchase { get; set; }
    public decimal TotalTax { get; set; }
    public List<PurchasesReportGroupDto> Groups { get; set; } = new();
    public List<TopSupplierDto> TopSuppliers { get; set; } = new();
}

public class PurchasesReportGroupDto
{
    public string Period { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public decimal Total { get; set; }
    public decimal Tax { get; set; }
}

public class TopSupplierDto
{
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public int PurchaseCount { get; set; }
    public decimal TotalPurchased { get; set; }
}

public class InventoryReportDto
{
    public int TotalProducts { get; set; }
    public int ActiveProducts { get; set; }
    public int LowStockProducts { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public List<InventoryReportCategoryDto> ByCategory { get; set; } = new();
    public List<LowStockProductDto> LowStockItems { get; set; } = new();
}

public class InventoryReportCategoryDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public decimal TotalValue { get; set; }
}

public class LowStockProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public int MinStock { get; set; }
    public decimal CostPrice { get; set; }
}
