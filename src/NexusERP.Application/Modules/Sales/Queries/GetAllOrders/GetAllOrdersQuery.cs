using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Sales.DTOs;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Sales.Queries.GetAllOrders;

public class GetAllOrdersQuery : PaginationRequest, IRequest<ApiResponse<PagedResult<OrderDto>>>
{
    public OrderType? OrderType { get; set; }
    public OrderStatus? Status { get; set; }
}

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, ApiResponse<PagedResult<OrderDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllOrdersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<OrderDto>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var allOrders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);
        var query = allOrders.AsEnumerable();

        if (request.OrderType.HasValue)
            query = query.Where(o => o.OrderType == request.OrderType.Value);

        if (request.Status.HasValue)
            query = query.Where(o => o.Status == request.Status.Value);

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
            .Select(o => new OrderDto
            {
                Id = o.Id,
                OrderNumber = o.OrderNumber,
                OrderType = o.OrderType,
                ClientId = o.ClientId,
                SupplierId = o.SupplierId,
                OrderDate = o.OrderDate,
                ExpectedDate = o.ExpectedDate,
                Status = o.Status,
                Subtotal = o.Subtotal,
                TaxAmount = o.TaxAmount,
                Total = o.Total,
                Notes = o.Notes,
                ItemCount = 0
            })
            .ToList();

        return ApiResponse<PagedResult<OrderDto>>.Ok(new PagedResult<OrderDto>(items, totalCount, request.Page, request.PageSize));
    }
}
