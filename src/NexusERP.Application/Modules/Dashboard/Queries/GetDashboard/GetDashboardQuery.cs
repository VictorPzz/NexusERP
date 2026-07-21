using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Dashboard.DTOs;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Dashboard.Queries.GetDashboard;

public class GetDashboardQuery : IRequest<ApiResponse<DashboardDto>>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, ApiResponse<DashboardDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDashboardQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<DashboardDto>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
    {
        var allSales = await _unitOfWork.Sales.GetAllAsync(cancellationToken);
        var allPurchases = await _unitOfWork.Purchases.GetAllAsync(cancellationToken);
        var allClients = await _unitOfWork.Clients.GetAllAsync(cancellationToken);
        var allProducts = await _unitOfWork.Products.GetAllAsync(cancellationToken);
        var allOrders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);
        var allPurchaseOrders = await _unitOfWork.PurchaseOrders.GetAllAsync(cancellationToken);

        var salesInRange = allSales.Where(s =>
            s.SaleDate >= request.StartDate && s.SaleDate <= request.EndDate &&
            s.Status != SaleStatus.cancelled).ToList();

        var purchasesInRange = allPurchases.Where(p =>
            p.PurchaseDate >= request.StartDate && p.PurchaseDate <= request.EndDate &&
            p.Status != PurchaseStatus.cancelled).ToList();

        var totalSales = salesInRange.Sum(s => s.Total);
        var totalPurchases = purchasesInRange.Sum(p => p.Total);

        var topProducts = salesInRange
            .SelectMany(s => s.Details)
            .GroupBy(d => d.ProductId)
            .Select(g => new TopProductDto
            {
                ProductId = g.Key,
                ProductName = allProducts.FirstOrDefault(p => p.Id == g.Key)?.Name ?? "Unknown",
                ProductCode = allProducts.FirstOrDefault(p => p.Id == g.Key)?.Code ?? "N/A",
                QuantitySold = g.Sum(d => d.Quantity),
                TotalRevenue = g.Sum(d => d.Total)
            })
            .OrderByDescending(t => t.TotalRevenue)
            .Take(10)
            .ToList();

        var dailyMetrics = Enumerable.Range(0, (request.EndDate - request.StartDate).Days + 1)
            .Select(i => request.StartDate.AddDays(i))
            .Select(date => new DailyMetricDto
            {
                Date = date,
                Sales = salesInRange.Where(s => s.SaleDate.Date == date.Date).Sum(s => s.Total),
                Purchases = purchasesInRange.Where(p => p.PurchaseDate.Date == date.Date).Sum(p => p.Total)
            })
            .ToList();

        var lowStockProducts = allProducts.Count(p => p.IsActive && p.MinStock > 0 &&
            !allProducts.Any(inventoryProd => inventoryProd.Id == p.Id));

        var dto = new DashboardDto
        {
            TotalSales = totalSales,
            TotalPurchases = totalPurchases,
            TotalRevenue = totalSales - totalPurchases,
            TotalClients = allClients.Count,
            TotalProducts = allProducts.Count(p => p.IsActive),
            LowStockProducts = allProducts.Count(p => p.IsActive && p.MinStock > 0),
            PendingOrders = allOrders.Count(o => o.Status == OrderStatus.pending || o.Status == OrderStatus.confirmed),
            PendingPurchases = allPurchaseOrders.Count(o => o.Status == PurchaseOrderStatus.pending || o.Status == PurchaseOrderStatus.approved),
            TopProducts = topProducts,
            DailyMetrics = dailyMetrics
        };

        return ApiResponse<DashboardDto>.Ok(dto);
    }
}
