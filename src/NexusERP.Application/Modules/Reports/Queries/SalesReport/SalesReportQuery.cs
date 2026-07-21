using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Reports.DTOs;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Reports.Queries.SalesReport;

public class SalesReportQuery : ReportFilters, IRequest<ApiResponse<SalesReportDto>>
{
}

public class SalesReportQueryHandler : IRequestHandler<SalesReportQuery, ApiResponse<SalesReportDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SalesReportQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<SalesReportDto>> Handle(SalesReportQuery request, CancellationToken cancellationToken)
    {
        var allSales = await _unitOfWork.Sales.GetAllAsync(cancellationToken);
        var allClients = await _unitOfWork.Clients.GetAllAsync(cancellationToken);

        var salesInRange = allSales.Where(s =>
            s.SaleDate >= request.StartDate && s.SaleDate <= request.EndDate &&
            s.Status != SaleStatus.cancelled).ToList();

        var groups = GroupSales(salesInRange, request.GroupBy);

        var topClients = salesInRange
            .GroupBy(s => s.ClientId)
            .Select(g => new TopClientDto
            {
                ClientId = g.Key,
                ClientName = allClients.FirstOrDefault(c => c.Id == g.Key)?.Person?.FirstName + " " +
                             allClients.FirstOrDefault(c => c.Id == g.Key)?.Person?.LastName ?? "Unknown",
                PurchaseCount = g.Count(),
                TotalSpent = g.Sum(s => s.Total)
            })
            .OrderByDescending(c => c.TotalSpent)
            .Take(10)
            .ToList();

        var dto = new SalesReportDto
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalSales = salesInRange.Sum(s => s.Total),
            TotalTransactions = salesInRange.Count,
            AverageSale = salesInRange.Count > 0 ? salesInRange.Average(s => s.Total) : 0,
            TotalTax = salesInRange.Sum(s => s.TaxAmount),
            Groups = groups,
            TopClients = topClients
        };

        return ApiResponse<SalesReportDto>.Ok(dto);
    }

    private List<SalesReportGroupDto> GroupSales(List<Domain.Entities.Sales.Sale> sales, string? groupBy)
    {
        return groupBy?.ToLower() switch
        {
            "week" => sales.GroupBy(s => System.Globalization.CultureInfo.CurrentCulture.Calendar
                    .GetWeekOfYear(s.SaleDate, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday))
                .Select(g => new SalesReportGroupDto
                {
                    Period = $"Week {g.Key}",
                    TransactionCount = g.Count(),
                    Total = g.Sum(s => s.Total),
                    Tax = g.Sum(s => s.TaxAmount)
                }).ToList(),
            "month" => sales.GroupBy(s => s.SaleDate.ToString("yyyy-MM"))
                .Select(g => new SalesReportGroupDto
                {
                    Period = g.Key,
                    TransactionCount = g.Count(),
                    Total = g.Sum(s => s.Total),
                    Tax = g.Sum(s => s.TaxAmount)
                }).ToList(),
            _ => sales.GroupBy(s => s.SaleDate.Date)
                .Select(g => new SalesReportGroupDto
                {
                    Period = g.Key.ToString("yyyy-MM-dd"),
                    TransactionCount = g.Count(),
                    Total = g.Sum(s => s.Total),
                    Tax = g.Sum(s => s.TaxAmount)
                }).ToList()
        };
    }
}
