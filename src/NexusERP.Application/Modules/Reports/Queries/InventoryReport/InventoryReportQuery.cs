using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Reports.DTOs;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Reports.Queries.InventoryReport;

public class InventoryReportQuery : IRequest<ApiResponse<InventoryReportDto>>
{
    public int? CategoryId { get; set; }
    public bool? LowStock { get; set; }
}

public class InventoryReportQueryHandler : IRequestHandler<InventoryReportQuery, ApiResponse<InventoryReportDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public InventoryReportQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<InventoryReportDto>> Handle(InventoryReportQuery request, CancellationToken cancellationToken)
    {
        var allProducts = await _unitOfWork.Products.GetAllAsync(cancellationToken);
        var allCategories = await _unitOfWork.Categories.GetAllAsync(cancellationToken);

        var query = allProducts.AsQueryable();

        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);

        var products = query.ToList();

        var lowStockItems = products
            .Where(p => p.IsActive && p.MinStock > 0)
            .OrderBy(p => p.CostPrice)
            .Select(p => new LowStockProductDto
            {
                ProductId = p.Id,
                ProductName = p.Name,
                ProductCode = p.Code,
                MinStock = p.MinStock,
                CostPrice = p.CostPrice
            })
            .ToList();

        if (request.LowStock == true)
            lowStockItems = lowStockItems.Where(p => p.MinStock > 0).ToList();

        var byCategory = products
            .GroupBy(p => p.CategoryId)
            .Select(g => new InventoryReportCategoryDto
            {
                CategoryId = g.Key,
                CategoryName = allCategories.FirstOrDefault(c => c.Id == g.Key)?.Name ?? "Unknown",
                ProductCount = g.Count(),
                TotalValue = g.Sum(p => p.CostPrice)
            })
            .OrderByDescending(c => c.TotalValue)
            .ToList();

        var dto = new InventoryReportDto
        {
            TotalProducts = allProducts.Count,
            ActiveProducts = allProducts.Count(p => p.IsActive),
            LowStockProducts = lowStockItems.Count,
            TotalInventoryValue = products.Where(p => p.IsActive).Sum(p => p.CostPrice),
            ByCategory = byCategory,
            LowStockItems = lowStockItems
        };

        return ApiResponse<InventoryReportDto>.Ok(dto);
    }
}
