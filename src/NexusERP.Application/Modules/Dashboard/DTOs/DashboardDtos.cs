namespace NexusERP.Application.Modules.Dashboard.DTOs;

public class DashboardDto
{
    public decimal TotalSales { get; set; }
    public decimal TotalPurchases { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalClients { get; set; }
    public int TotalProducts { get; set; }
    public int LowStockProducts { get; set; }
    public int PendingOrders { get; set; }
    public int PendingPurchases { get; set; }
    public List<TopProductDto> TopProducts { get; set; } = new();
    public List<DailyMetricDto> DailyMetrics { get; set; } = new();
}

public class TopProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class DailyMetricDto
{
    public DateTime Date { get; set; }
    public decimal Sales { get; set; }
    public decimal Purchases { get; set; }
}
