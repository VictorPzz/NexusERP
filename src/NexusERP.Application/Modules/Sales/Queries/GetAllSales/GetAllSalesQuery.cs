using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Sales.DTOs;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Sales.Queries.GetAllSales;

public class GetAllSalesQuery : PaginationRequest, IRequest<ApiResponse<PagedResult<SaleDto>>>
{
    public SaleStatus? Status { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
}

public class GetAllSalesQueryHandler : IRequestHandler<GetAllSalesQuery, ApiResponse<PagedResult<SaleDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllSalesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<SaleDto>>> Handle(GetAllSalesQuery request, CancellationToken cancellationToken)
    {
        var allSales = await _unitOfWork.Sales.GetAllAsync(cancellationToken);
        var query = allSales.AsEnumerable();

        if (request.Status.HasValue)
            query = query.Where(s => s.Status == request.Status.Value);

        if (request.PaymentStatus.HasValue)
            query = query.Where(s => s.PaymentStatus == request.PaymentStatus.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(s => s.SaleNumber.ToLower().Contains(search));
        }

        var totalCount = query.Count();
        var items = query
            .OrderByDescending(s => s.SaleDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new SaleDto
            {
                Id = s.Id,
                SaleNumber = s.SaleNumber,
                ClientId = s.ClientId,
                SaleDate = s.SaleDate,
                Status = s.Status,
                PaymentStatus = s.PaymentStatus,
                Subtotal = s.Subtotal,
                TaxAmount = s.TaxAmount,
                DiscountAmount = s.DiscountAmount,
                Total = s.Total,
                Notes = s.Notes,
                ItemCount = 0
            })
            .ToList();

        return ApiResponse<PagedResult<SaleDto>>.Ok(new PagedResult<SaleDto>(items, totalCount, request.Page, request.PageSize));
    }
}
