using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Purchases.DTOs;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Purchases.Queries.GetAllPurchaseOrders;

public class GetAllPurchaseOrdersQuery : PaginationRequest, IRequest<ApiResponse<PagedResult<PurchaseOrderDto>>>
{
    public PurchaseOrderStatus? Status { get; set; }
    public int? SupplierId { get; set; }
}

public class GetAllPurchaseOrdersQueryHandler : IRequestHandler<GetAllPurchaseOrdersQuery, ApiResponse<PagedResult<PurchaseOrderDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllPurchaseOrdersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<PurchaseOrderDto>>> Handle(GetAllPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        var allOrders = await _unitOfWork.PurchaseOrders.GetAllAsync(cancellationToken);
        var query = allOrders.AsEnumerable();

        if (request.Status.HasValue)
            query = query.Where(o => o.Status == request.Status.Value);

        if (request.SupplierId.HasValue)
            query = query.Where(o => o.SupplierId == request.SupplierId.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(o => o.OrderNumber.ToLower().Contains(search));
        }

        var totalCount = query.Count();
        var items = query
            .OrderByDescending(o => o.OrderDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new PurchaseOrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                SupplierId = o.SupplierId,
                OrderDate = o.OrderDate,
                ExpectedDate = o.ExpectedDate,
                Status = o.Status,
                Subtotal = o.Subtotal,
                TaxAmount = o.TaxAmount,
                Total = o.Total,
                Notes = o.Notes,
                ApprovedBy = o.ApprovedBy,
                ApprovedAt = o.ApprovedAt,
                ItemCount = 0
            })
            .ToList();

        return ApiResponse<PagedResult<PurchaseOrderDto>>.Ok(new PagedResult<PurchaseOrderDto>(items, totalCount, request.Page, request.PageSize));
    }
}
