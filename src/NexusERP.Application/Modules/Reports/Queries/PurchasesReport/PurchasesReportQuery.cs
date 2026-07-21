using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Reports.DTOs;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Reports.Queries.PurchasesReport;

public class PurchasesReportQuery : ReportFilters, IRequest<ApiResponse<PurchasesReportDto>>
{
}

public class PurchasesReportQueryHandler : IRequestHandler<PurchasesReportQuery, ApiResponse<PurchasesReportDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public PurchasesReportQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PurchasesReportDto>> Handle(PurchasesReportQuery request, CancellationToken cancellationToken)
    {
        var allPurchases = await _unitOfWork.Purchases.GetAllAsync(cancellationToken);
        var allSuppliers = await _unitOfWork.Suppliers.GetAllAsync(cancellationToken);

        var purchasesInRange = allPurchases.Where(p =>
            p.PurchaseDate >= request.StartDate && p.PurchaseDate <= request.EndDate &&
            p.Status != PurchaseStatus.cancelled).ToList();

        var groups = GroupPurchases(purchasesInRange, request.GroupBy);

        var topSuppliers = purchasesInRange
            .GroupBy(p => p.SupplierId)
            .Select(g => new TopSupplierDto
            {
                SupplierId = g.Key,
                SupplierName = allSuppliers.FirstOrDefault(s => s.Id == g.Key)?.Person?.FirstName + " " +
                               allSuppliers.FirstOrDefault(s => s.Id == g.Key)?.Person?.LastName ?? "Unknown",
                PurchaseCount = g.Count(),
                TotalPurchased = g.Sum(p => p.Total)
            })
            .OrderByDescending(s => s.TotalPurchased)
            .Take(10)
            .ToList();

        var dto = new PurchasesReportDto
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalPurchases = purchasesInRange.Sum(p => p.Total),
            TotalTransactions = purchasesInRange.Count,
            AveragePurchase = purchasesInRange.Count > 0 ? purchasesInRange.Average(p => p.Total) : 0,
            TotalTax = purchasesInRange.Sum(p => p.TaxAmount),
            Groups = groups,
            TopSuppliers = topSuppliers
        };

        return ApiResponse<PurchasesReportDto>.Ok(dto);
    }

    private List<PurchasesReportGroupDto> GroupPurchases(List<Domain.Entities.Purchases.Purchase> purchases, string? groupBy)
    {
        return groupBy?.ToLower() switch
        {
            "week" => purchases.GroupBy(p => System.Globalization.CultureInfo.CurrentCulture.Calendar
                    .GetWeekOfYear(p.PurchaseDate, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday))
                .Select(g => new PurchasesReportGroupDto
                {
                    Period = $"Week {g.Key}",
                    TransactionCount = g.Count(),
                    Total = g.Sum(p => p.Total),
                    Tax = g.Sum(p => p.TaxAmount)
                }).ToList(),
            "month" => purchases.GroupBy(p => p.PurchaseDate.ToString("yyyy-MM"))
                .Select(g => new PurchasesReportGroupDto
                {
                    Period = g.Key,
                    TransactionCount = g.Count(),
                    Total = g.Sum(p => p.Total),
                    Tax = g.Sum(p => p.TaxAmount)
                }).ToList(),
            _ => purchases.GroupBy(p => p.PurchaseDate.Date)
                .Select(g => new PurchasesReportGroupDto
                {
                    Period = g.Key.ToString("yyyy-MM-dd"),
                    TransactionCount = g.Count(),
                    Total = g.Sum(p => p.Total),
                    Tax = g.Sum(p => p.TaxAmount)
                }).ToList()
        };
    }
}
